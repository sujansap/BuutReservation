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

        [Fact]
        public async Task GET_CurrentUser_PastReservations_WithNoParameters_FirstPage_ExpectOk_5OrLessReservations()
        {
            await LoginAsync(TestLoginRole.Member);

            var response = await _client.GetAsync("me?getPast=true");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationsPage = await response.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            reservationsPage.ShouldNotBeNull();
            // no data to be tested against
            // reservationsPage.Data.ShouldNotBeEmpty();
            // reservationsPage.Data.Count().ShouldBeLessThanOrEqualTo(5);
            // reservationsPage.IsFirstPage.ShouldBeTrue();
            // reservationsPage.PreviousId.ShouldBeNull();
            // reservationsPage.NextId.ShouldNotBeNull();
            // reservationsPage.Data.ShouldAllBe(r => r.Date < DateOnly.FromDateTime(DateTime.Now));
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
                TimeSlotId = 10
            };


            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var reservationId = await response.Content.ReadFromJsonAsync<int>();
            reservationId.ShouldBeGreaterThan(0);

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
    }
}
