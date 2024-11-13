using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Rise.Shared.Reservations;
using Rise.Shared.TimeSlots;
using Shouldly;

namespace Rise.Client.Tests
{
    [TestFixture]
    public class ReservationDetailsTest : CustomPageTest
    {
        private const string UserReservationDetailsUrl = "/reservations/1";
        private const string InvalidReservationDetailsUrl = "/reservations/100";


        private async Task MockReservationDetailsApi(ReservationDetailsDto? response = null, int status = 200)
        {
            await Page.RouteAsync(new Regex("^.*/api/Reservation/\\d+"), async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = status,
                    ContentType = "application/json",
                    Body = response != null ? System.Text.Json.JsonSerializer.Serialize(response) : string.Empty
                });
            });
        }

        [Test]
        public async Task ShowsReservationDetails()
        {
            var reservationDetails = new ReservationDetailsDto
            {
                Id = 1,
                Date = DateOnly.Parse("2024/10/30"),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                BatteryType = "Lithium-Ion"
            };

            await MockReservationDetailsApi(reservationDetails);
            await Page.GotoAsync(UserReservationDetailsUrl);

            await Page.WaitForRequestAsync(request => request.Url.Contains("api/Reservation/1"));

            var dateText = await Page.GetByTestId("reservation-date").TextContentAsync();

            var datePattern = @"\b\d{2}/\d{2}/\d{4}\b";
            var match = Regex.Match(dateText, datePattern);

            match.Success.ShouldBeTrue();
            var formattedDate = match.Value.Replace("/", "-");

            formattedDate.ShouldBe(reservationDetails.Date.ToString("dd-MM-yyyy"));

            var boatNameText = await Page.GetByTestId("reservation-boat").TextContentAsync();
            boatNameText.ShouldContain(reservationDetails.BoatPersonalName);

            var timeText = await Page.GetByTestId("reservation-time").TextContentAsync();
            timeText.ShouldContain($"{reservationDetails.Start:HH:mm} - {reservationDetails.End:HH:mm}");

            var batteryText = await Page.GetByTestId("reservation-battery").TextContentAsync();
            batteryText.ShouldContain(reservationDetails.BatteryType);

            var batteryMentorText = await Page.GetByTestId("reservation-battery-mentor").TextContentAsync();
            batteryMentorText.ShouldContain(reservationDetails.MentorName);
        }
        [Test]
        public async Task ShowsNotFoundErrorForNonExistentReservation()
        {
            await MockReservationDetailsApi(null, status: 404);
            await Page.GotoAsync(InvalidReservationDetailsUrl);

            var errorMessage = Page.Locator("text='Response status code does not indicate success: 404 (Not Found).'");

            await Expect(errorMessage).ToBeVisibleAsync(new() { Timeout = 5000 });

            Assert.IsTrue(await errorMessage.IsVisibleAsync());
        }




    }
}