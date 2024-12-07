using System.Text.Json;
using System.Text.RegularExpressions;
using Rise.Shared.Reservations;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Reservations
{
    public class ReservationDetailsPageTest : CustomAuthenticatedPageTest
    {
        private const string UserReservationDetailsUrl = "/reservations/1";
        private const string InvalidReservationDetailsUrl = "/reservations/100";

        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Member);
        }

        private async Task MockReservationDetailsApi(ReservationDetailsDto? response = null, int status = 200)
        {
            await Page.RouteAsync(new Regex("^.*/api/Reservation/\\d+"), async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = status,
                    ContentType = "application/json",
                    Body = response != null ? JsonSerializer.Serialize(response) : string.Empty
                });
            });
        }

        [Test]
        public async Task ShowsReservationDetails()
        {
            ReservationDetailsDto reservationDetails = new()
            {
                Id = 1,
                Date = new DateOnly(2024, 10, 30),
                Start = new TimeOnly(10, 0, 0),
                End = new TimeOnly(13, 0, 0),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                BatteryType = "Lithium-Ion"
            };

            await MockReservationDetailsApi(reservationDetails);
            await NavigateToUrl(UserReservationDetailsUrl);

            await Expect(Page.GetByTestId("reservation-details-date")).ToContainTextAsync(reservationDetails.Date.ToString("dd/MM/yyyy"));
            await Expect(Page.GetByTestId("reservation-details-boat")).ToContainTextAsync(reservationDetails.BoatPersonalName);
            await Expect(Page.GetByTestId("reservation-details-time")).ToContainTextAsync($"{reservationDetails.Start:HH:mm} - {reservationDetails.End:HH:mm}");
            await Expect(Page.GetByTestId("reservation-details-battery")).ToContainTextAsync(reservationDetails.BatteryType);
            await Expect(Page.GetByTestId("reservation-details-battery-mentor")).ToContainTextAsync(reservationDetails.MentorName);
        }

        [Test]
        public async Task ShowsNotFoundErrorForNonExistentReservation()
        {
            await MockReservationDetailsApi(null, status: 404);
            await NavigateToUrl(UserReservationDetailsUrl);

            var errorMessage = Page.Locator("text='Response status code does not indicate success: 404 (Not Found).'");

            await Expect(errorMessage).ToBeVisibleAsync(new() { Timeout = 30000 });
        }




    }
}