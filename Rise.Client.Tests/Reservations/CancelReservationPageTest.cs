using System.Text.RegularExpressions;
using Rise.Shared.Reservations;

namespace Rise.Client.Tests;

[TestFixture]
public class CancelReservationTest : CustomPageTest
{
    private const string UserReservationDetailsUrl = "/reservations/1";
    private const string ReservationsListUrl = "/reservations?CurrentTab=reservations";

    private async Task MockCancelReservationApi(int reservationId, int status = 200)
    {
        await Page.RouteAsync($"**/api/Reservation/{reservationId}/cancel", async route =>
        {
            await route.FulfillAsync(new()
            {
                Status = status,
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
            Date = DateOnly.Parse("2024/12/01"),
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
        await Page.GotoAsync(UserReservationDetailsUrl);
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();

        // Assert: Verify redirect and status update in the list
        await Expect(Page).ToHaveURLAsync(ReservationsListUrl);
        var reservationStatus = Page.GetByTestId($"reservation-{reservationDetails.Id}-status");
        await Expect(reservationStatus).ToHaveTextAsync("Geannuleerd");
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
        await Page.GotoAsync($"/reservations/{reservationDetails.Id}");

        // Assert: Verify the "Annuleer" button is disabled
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await Expect(cancelButton).ToBeDisabledAsync();
    }

    [Test]
    public async Task ShowMessageForCancelledReservation()
    {
        // Arrange: Mock reservation details for an already canceled reservation
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

        // Act: Go to reservation details
        await Page.GotoAsync($"/reservations/{reservationDetails.Id}");

        // Assert: Verify "Reservation Cancelled" message is displayed
        var cancelledMessage = Page.GetByTestId("reservation-cancelled-message");
        await Expect(cancelledMessage).ToBeVisibleAsync();
        await Expect(cancelledMessage).ToHaveTextAsync("Deze reservatie is geannuleerd. Je kan de details niet bekijken.");

        // Assert: Ensure "Annuleer" button is not visible
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await Expect(cancelButton).ToHaveCountAsync(0);
    }

    [Test]
    public async Task ShowErrorOnCancelApiFailure()
    {
        // Arrange: Mock reservation details and a failing cancel API
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 4,
            Date = DateOnly.Parse("2024/12/10"),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 4,
            BoatPersonalName = "Ocean Wave",
            MentorName = "Alice Johnson",
            BatteryType = "Lithium-Ion",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);
        await MockCancelReservationApi(reservationDetails.Id, status: 500); // Internal server error

        // Act: Attempt to cancel the reservation
        await Page.GotoAsync($"/reservations/{reservationDetails.Id}");
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();

        // Assert: Verify error message is displayed
        var errorMessage = Page.GetByTestId("reservation-cancel-error");
        await Expect(errorMessage).ToBeVisibleAsync();
        await Expect(errorMessage).ToHaveTextAsync("Er is een fout opgetreden bij het annuleren van deze reservatie. Probeer opnieuw.");
    }
}

