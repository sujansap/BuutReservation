using Rise.Server.Tests.Fixtures;
using Shouldly;
using System.Net.Http.Json;
using System.Net;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;

namespace Rise.Server.Tests.Controllers
{
    public class ReservationControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "Reservation")
    {
        [Fact]
        public async Task GET_CurrentUser_UpcomingReservations_WithNoParameters_FirstPage_ExpectOk_5OrLessReservations()
        {
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
        }

        // baken zelf de range af van de reservations van een maand geleden + 5
        [Fact]
        public async Task GET_CurrentUser_PastReservations_WithNoParameters_FirstPage_ExpectOk_5OrLessReservations()
        {
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
            var today = DateOnly.FromDateTime(DateTime.Now);
            var oneMonthAgo = today.AddMonths(-1);

            // Verify all returned reservations are from the past
            reservationsPage.Data.ShouldAllBe(r => r.Date < today);

            reservationsPage.Data.ShouldAllBe(r => r.Date >= oneMonthAgo.AddDays(5));
        }

        [Fact]
        public async Task GET_CurrentUser_PastReservations_NextPage_ExpectOk_5OrLessReservations()
        {
            var firstResponse = await _client.GetAsync("me?getPast=true");
            firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var firstPage = await firstResponse.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            firstPage.ShouldNotBeNull();
            firstPage.NextId.ShouldNotBeNull();

            var nextResponse = await _client.GetAsync($"me?getPast=true&cursor={firstPage.NextId}&isNextPage=true");
            nextResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            var nextPage = await nextResponse.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            var today = DateOnly.FromDateTime(DateTime.Now);
            var oneMonthAgo = today.AddMonths(-1);
            nextPage.ShouldNotBeNull();
            nextPage.Data.ShouldNotBeEmpty();
            nextPage.Data.Count().ShouldBeLessThanOrEqualTo(5);
            nextPage.IsFirstPage.ShouldBeFalse();
            nextPage.PreviousId.ShouldNotBeNull();
            nextPage.Data.ShouldAllBe(r => r.Date < DateOnly.FromDateTime(DateTime.Now));

            nextPage.Data.First().Id.ShouldNotBe(firstPage.Data.First().Id);
            //maand geleden + 5 + 1 tot een maand geleden + 11 
            nextPage.Data.ShouldAllBe(r => r.Date >= oneMonthAgo.AddDays(5).AddDays(1) && r.Date <= oneMonthAgo.AddDays(11));
        }

        [Theory]
        [InlineData(7)]
        [InlineData(15)]
        public async Task GET_CurrentUser_UpcomingReservations_WithVaryingPageSize_ExpectOk_PageSizeAmountOrLessReservations(int pageSize)
        {
            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationsPage = await response.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            reservationsPage.ShouldNotBeNull();
            reservationsPage.Data.ShouldNotBeEmpty();
            reservationsPage.Data.Count().ShouldBeLessThanOrEqualTo(pageSize);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GET_CurrentUser_Reservations_WithInvalidPageSize_ExpectBadRequest(int pageSize)
        {
            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("invalid")]
        public async Task GET_CurrentUser_Reservations_WithInvalidPageSizeType_ExpectBadRequest(string pageSize)
        {
            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-56)]
        public async Task GET_CurrentUser_Reservations_WithInvalidNumber_ExpectBadRequest(int cursor)
        {
            var response = await _client.GetAsync($"me?cursor={cursor}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("invalid")]
        public async Task GET_CurrentUser_Reservations_WithInvalidType_ExpectBadRequest(string cursor)
        {
            var response = await _client.GetAsync($"me?cursor={cursor}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_CurrentUser_Reservations_NextPage_WithValidCursor_IsNextPageNull_ExpectBadRequest()
        {
            var response = await _client.GetAsync($"me?cursor=17");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task POST_CreateReservation_WithValidTimeSlot_ExpectCreated()
        {

            var request = new CreateReservationDto
            {
                TimeSlotId = 10
            };


            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var reservationId = await response.Content.ReadFromJsonAsync<int>();
            reservationId.ShouldBeGreaterThan(0);
        }


        [Fact]
        public async Task POST_CreateReservation_WithDuplicateTimeSlot_ExpectConflict()
        {

            var request = new CreateReservationDto
            {
                TimeSlotId = 33
            };

            //first a reservation should be created then it shouldn't be for the same user
            var response1 = await _client.PostAsJsonAsync("", request);
            response1.StatusCode.ShouldBe(HttpStatusCode.Created);
            var response2 = await _client.PostAsJsonAsync("", request);
            response2.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }


        [Fact]
        public async Task POST_CreateReservation_WithQueryParameters_ExpectBadRequest()
        {
            var request = new CreateReservationDto
            {
                TimeSlotId = 1
            };

            var response = await _client.PostAsJsonAsync("?badrequest=true", request);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_CreateReservation_WithNoAvailableBoats_ExpectConflict()
        {
            var request = new CreateReservationDto
            {
                TimeSlotId = 1
            };

            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task POST_CreateReservation_WithInvalidTimeSlot_ExpectNotFound()
        {
            var request = new CreateReservationDto
            {
                TimeSlotId = -1
            };


            var response = await _client.PostAsJsonAsync("", request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_CreateReservation_WithMissingTimeSlotId_ExpectBadRequest()
        {
            var request = new CreateReservationDto();

            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_ReservationDetails_WithExistingId_ExpectOk()
        {

            var existingId = 1;
            var response = await _client.GetAsync($"{existingId}");


            response.StatusCode.ShouldBe(HttpStatusCode.OK);


            var reservationDetails = await response.Content.ReadFromJsonAsync<ReservationDetailsDto>();
            reservationDetails.ShouldNotBeNull();
            reservationDetails.Id.ShouldBe(existingId);
        }

        [Fact]
        public async Task GET_ReservationDetails_WithNonExistentId_ExpectNotFound()
        {

            var nonExistentId = 9999;
            var response = await _client.GetAsync($"{nonExistentId}");


            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("abc")]
        [InlineData("@!#")]
        public async Task GET_ReservationDetails_WithInvalidId_ExpectBadRequest(string invalidId)
        {

            var response = await _client.GetAsync($"{invalidId}");


            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
    }
}
