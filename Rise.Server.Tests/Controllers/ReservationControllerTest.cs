using Rise.Server.Tests.Fixtures;
using Shouldly;
using System.Net.Http.Json;
using System.Net;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Rise.Server.Tests.Utils;

namespace Rise.Server.Tests.Controllers
{
    public class ReservationControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "Reservation")
    {
        [Theory]
        [InlineData("me", TestLoginRole.Guest, "GET")]
        [InlineData("", TestLoginRole.Guest, "POST")]
        [InlineData("1", TestLoginRole.Guest, "GET")]
        public async Task Call_ReservationController_Endpoints_ExpectForbidden(string url, TestLoginRole testLoginRole, string httpMethod)
        {
            await LoginAsync(testLoginRole);

            HttpResponseMessage? response = httpMethod switch
            {
                "GET" => await _client.GetAsync(url),
                "POST" => await _client.PostAsJsonAsync(url, new object()),
                _ => null,
            };
            ;
            response?.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

            Logout();
        }

        [Theory]
        [InlineData("me", "GET")]
        [InlineData("", "POST")]
        [InlineData("1", "GET")]
        public async Task Call_ReservationController_Endpoints_ExpectUnauthorized(string url, string httpMethod)
        {

            HttpResponseMessage? response = httpMethod switch
            {
                "GET" => await _client.GetAsync(url),
                "POST" => await _client.PostAsJsonAsync(url, new object()),
                _ => null,
            };
            ;
            response?.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GET_CurrentUser_UpcomingReservations_WithNoParameters_FirstPage_ExpectOk_5OrLessReservations()
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync("me");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationsPage = await response.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            reservationsPage.ShouldNotBeNull();
            reservationsPage.Data.ShouldNotBeEmpty();
            reservationsPage.Data.Count().ShouldBeLessThanOrEqualTo(5);
            reservationsPage.IsFirstPage.ShouldBeTrue();
            reservationsPage.PreviousId.ShouldBeNull();
            reservationsPage.NextId.ShouldNotBeNull();
            reservationsPage.Data.ShouldAllBe(r => r.Date >= DateOnly.FromDateTime(DateTime.Now));

            Logout();
        }

        // baken zelf de range af van de reservations van een maand geleden + 5
        [Fact]
        public async Task GET_CurrentUser_PastReservations_WithNoParameters_FirstPage_ExpectOk_5OrLessReservations()
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync("me?getPast=true");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationsPage = await response.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            reservationsPage.ShouldNotBeNull();
            reservationsPage.Data.ShouldNotBeEmpty();
            reservationsPage.Data.Count().ShouldBeLessThanOrEqualTo(5);
            reservationsPage.IsFirstPage.ShouldBeTrue();
            reservationsPage.PreviousId.ShouldBeNull();
            reservationsPage.NextId.ShouldNotBeNull();

            // Additional checks for past reservations
            var today = DateOnly.FromDateTime(DateTime.Today);
            var oneMonthAgo = today.AddMonths(-1);

            // Verify all returned reservations are from the past
            reservationsPage.Data.ShouldAllBe(r => r.Date < today);
            // start van 2 dagen geleden tot 7 dagen geleden
            reservationsPage.Data.ShouldAllBe(r => r.Date >= today.AddDays(-8) && r.Date <= today.AddDays(-2));
            Logout();
        }

        [Fact]
        public async Task GET_CurrentUser_PastReservations_NextPage_ExpectOk_5OrLessReservations()
        {
            await LoginAsync(TestLoginRole.Member);
            // Get first page
            var firstResponse = await _client.GetAsync("me?getPast=true");
            firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var firstPage = await firstResponse.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            firstPage.ShouldNotBeNull();
            firstPage.NextId.ShouldNotBeNull();

            // Store the last ID from first page to verify cursor implementation
            var lastIdFromFirstPage = firstPage.Data.Last().Id;

            // Get next page using cursor
            var nextResponse = await _client.GetAsync($"me?getPast=true&cursor={firstPage.NextId}&isNextPage=true");
            nextResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            var nextPage = await nextResponse.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            nextPage.ShouldNotBeNull();

            // de eerste van de lijst moet de cursor zijn van de vorige pagina
            nextPage.PreviousId.ShouldNotBe(lastIdFromFirstPage);  // Previous cursor should point to last item of first page
            nextPage.Data.First().Id.ShouldBeLessThan(lastIdFromFirstPage);  // Items should be ordered by ID descending

            var today = DateOnly.FromDateTime(DateTime.Now);
            var oneMonthAgo = today.AddMonths(-1);
            nextPage.Data.ShouldNotBeEmpty();
            nextPage.Data.Count().ShouldBeLessThanOrEqualTo(5);
            nextPage.IsFirstPage.ShouldBeFalse();
            nextPage.PreviousId.ShouldNotBeNull();
            nextPage.Data.ShouldAllBe(r => r.Date < DateOnly.FromDateTime(DateTime.Now));

            nextPage.Data.First().Id.ShouldNotBe(firstPage.Data.First().Id);
            // van 8 dagen geleden tot 13 dagen geleden
            // moet de cursor meegeven van de pagina
            DateOnly start = today.AddDays(-14);
            DateOnly end = today.AddDays(-9);

            nextPage.Data.ShouldAllBe(r => r.Date >= start && r.Date <= end,
                customMessage: $"Expected dates between {start} and {end}. " +
                $"Actual dates: {string.Join(", ", nextPage.Data.Select(r => r.Date))}");

            Logout();
        }

        [Theory]
        [InlineData(7)]
        [InlineData(15)]
        public async Task GET_CurrentUser_UpcomingReservations_WithVaryingPageSize_ExpectOk_PageSizeAmountOrLessReservations(int pageSize)
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationsPage = await response.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            reservationsPage.ShouldNotBeNull();
            reservationsPage.Data.ShouldNotBeEmpty();
            reservationsPage.Data.Count().ShouldBeLessThanOrEqualTo(pageSize);

            Logout();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GET_CurrentUser_Reservations_WithInvalidPageSize_ExpectBadRequest(int pageSize)
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }

        [Theory]
        [InlineData("invalid")]
        public async Task GET_CurrentUser_Reservations_WithInvalidPageSizeType_ExpectBadRequest(string pageSize)
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-56)]
        public async Task GET_CurrentUser_Reservations_WithInvalidNumber_ExpectBadRequest(int cursor)
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync($"me?cursor={cursor}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }

        [Theory]
        [InlineData("invalid")]
        public async Task GET_CurrentUser_Reservations_WithInvalidType_ExpectBadRequest(string cursor)
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync($"me?cursor={cursor}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }

        [Fact]
        public async Task GET_CurrentUser_Reservations_NextPage_WithValidCursor_IsNextPageNull_ExpectBadRequest()
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync($"me?cursor=17");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }


        [Fact]
        public async Task POST_CreateReservation_WithValidTimeSlot_ExpectCreated()
        {
            await LoginAsync(TestLoginRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = 63
            };


            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var reservationId = await response.Content.ReadFromJsonAsync<int>();
            reservationId.ShouldBeGreaterThan(0);

            Logout();
        }


        [Fact]
        public async Task POST_CreateReservation_WithDuplicateTimeSlot_ExpectConflict()
        {
            await LoginAsync(TestLoginRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = 33
            };

            //first a reservation should be created then it shouldn't be for the same user
            var response1 = await _client.PostAsJsonAsync("", request);
            response1.StatusCode.ShouldBe(HttpStatusCode.Created);
            var response2 = await _client.PostAsJsonAsync("", request);
            response2.StatusCode.ShouldBe(HttpStatusCode.Conflict);

            Logout();
        }


        [Fact]
        public async Task POST_CreateReservation_WithQueryParameters_ExpectBadRequest()
        {
            await LoginAsync(TestLoginRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = 1
            };

            var response = await _client.PostAsJsonAsync("?badrequest=true", request);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }

        [Fact]
        public async Task POST_CreateReservation_WithNoAvailableBoats_ExpectConflict()
        {
            await LoginAsync(TestLoginRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = 1
            };

            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.Conflict);

            Logout();
        }

        [Fact]
        public async Task POST_CreateReservation_WithInvalidTimeSlot_ExpectNotFound()
        {
            await LoginAsync(TestLoginRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = -1
            };


            var response = await _client.PostAsJsonAsync("", request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }

        [Fact]
        public async Task POST_CreateReservation_WithMissingTimeSlotId_ExpectBadRequest()
        {
            await LoginAsync(TestLoginRole.Member);

            var request = new CreateReservationDto();

            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }

        [Fact]
        public async Task GET_ReservationDetails_WithExistingId_ExpectOk()
        {
            await LoginAsync(TestLoginRole.Member);

            var existingId = 1;
            var response = await _client.GetAsync($"{existingId}");


            response.StatusCode.ShouldBe(HttpStatusCode.OK);


            var reservationDetails = await response.Content.ReadFromJsonAsync<ReservationDetailsDto>();
            reservationDetails.ShouldNotBeNull();
            reservationDetails.Id.ShouldBe(existingId);

            Logout();
        }

        [Fact]
        public async Task GET_ReservationDetails_WithNonExistentId_ExpectNotFound()
        {
            await LoginAsync(TestLoginRole.Member);

            var nonExistentId = 9999;
            var response = await _client.GetAsync($"{nonExistentId}");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);

            Logout();
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("abc")]
        [InlineData("@!#")]
        public async Task GET_ReservationDetails_WithInvalidId_ExpectBadRequest(string invalidId)
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync($"{invalidId}");

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            Logout();
        }
    }
}
