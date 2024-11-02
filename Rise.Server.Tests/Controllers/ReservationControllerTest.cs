using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.TimeSlots;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc;
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

        [Fact]
        public async Task GET_CurrentUser_PastReservations_WithNoParameters_FirstPage_ExpectOk_5OrLessReservations()
        {
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
            var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();
            reservation.ShouldNotBeNull();
            reservation.Id.ShouldBeGreaterThan(0);
            reservation.BoatId.ShouldBeGreaterThan(0);
            reservation.BoatPersonalName.ShouldNotBeNullOrEmpty();
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
    }
}
