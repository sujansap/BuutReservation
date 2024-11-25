using System.Text.RegularExpressions;
using Rise.Shared.Reservations;

namespace Rise.Client.Tests.Reservations;

[TestFixture]
public class CancelReservationTest : CustomPageTest
{
    private const string UserReservationDetailsUrl = "/reservations/41";
    private const string ReservationsListUrl = "/reservations?CurrentTab=reservations";

    private async Task MockCancelReservationApi(int reservationId, int status = 200)
    {
        await Page.RouteAsync($"**/api/Reservation/cancel/{reservationId}", async route =>
        {
            await route.FulfillAsync(new()
            {
                Status = 200,
                ContentType = "application/json",
                Body = string.Empty
            });
        });
    }

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
    public async Task CancelReservationSuccessfully()
    {
        // Arrange: Mock reservation details and cancel API
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 1,
            Date = DateOnly.Parse(DateTime.Now.AddDays(5).ToString("yyyy/MM/dd")), //is cancelbaar
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 1,
            BoatPersonalName = "Limba",
            MentorName = "John Doe",
            BatteryType = "Lithium-Ion",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);
        await MockCancelReservationApi(reservationDetails.Id);

        // Act: Go to reservation details and cancel
        await InitNavigationToUrl(UserReservationDetailsUrl);
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();

        // Assert: Verify redirect and status update in the list
        await Expect(Page).ToHaveURLAsync(ReservationsListUrl);
        // var reservationStatus = Page.GetByTestId("reservationList-cancelled");
        // await Expect(reservationStatus).ToHaveTextAsync("Cancelled");
    }

    [Test]
    public async Task PreventCancelReservationWithin2Days()
    {
        // Arrange: Mock reservation details for a non-cancelable reservation
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 2,
            Date = DateOnly.Parse(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd")), // 1 day away
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 2,
            BoatPersonalName = "Swan",
            MentorName = "Jane Doe",
            BatteryType = "Nickel-Cadmium",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);

        // Act: Go to reservation details
        await InitNavigationToUrl($"/reservations/{reservationDetails.Id}");

        // Assert: Verify the "Annuleer" button is disabled
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();
        await Expect(Page).ToHaveURLAsync($"/reservations/{reservationDetails.Id}");
        // await Expect(cancelButton).ToBeDisabledAsync();

    }

    [Test]
    public async Task ShowMessageForCancelledReservation()
    {
        // Arrange:
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 3,
            Date = DateOnly.Parse("2024/12/05"),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 3,
            BoatPersonalName = "Sea Breeze",
            MentorName = "John Smith",
            BatteryType = "Lead-Acid",
            IsDeleted = true
        };

        await MockReservationDetailsApi(reservationDetails);

        // Act: 
        await InitNavigationToUrl($"/reservations/{reservationDetails.Id}");

        // Assert:
        var cancelledMessage = Page.GetByTestId("cancel-reservation-geannuleerd");
        await Expect(cancelledMessage).ToBeVisibleAsync();
        await Expect(cancelledMessage).ToHaveTextAsync("Deze reservatie is geannuleerd. Je kan de details niet bekijken.");
    }

}

