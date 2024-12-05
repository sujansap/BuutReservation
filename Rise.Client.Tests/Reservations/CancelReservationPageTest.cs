using System.Text.RegularExpressions;
using Rise.Shared.Reservations;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Reservations;

[TestFixture]
public class CancelReservationTest : CustomAuthenticatedPageTest
{
    [SetUp]
    public async Task SetUpAsync()
    {
        base.GlobalSetUp();
        await LoginAsync(UserRole.Member);
    }

    [TearDown]
    public async Task TearDownAsync()
    {
        await LogoutAsync();
        await base.TearDown();
    }

    private const string UserReservationDetailsUrl = "/reservations/41";
    private const string ReservationsListUrl = "/reservations?CurrentTab=reservations";

    private async Task MockCancelReservationApi(int reservationId, int status = 200)
    {
        await Page.RouteAsync($"**/api/Reservation/cancel/{reservationId}", async route =>
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
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 1,
            Date = DateOnly.Parse(DateTime.Now.AddDays(5).ToString("yyyy/MM/dd")),
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

        await NavigateToUrl(UserReservationDetailsUrl);
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();

        await Expect(Page).ToHaveURLAsync(ReservationsListUrl);
    }

    [Test]
    public async Task PreventCancelReservationWithin2Days()
    {
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


        await NavigateToUrl($"/reservations/{reservationDetails.Id}");


        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();
        await Expect(Page).ToHaveURLAsync($"/reservations/{reservationDetails.Id}");


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

        await NavigateToUrl($"/reservations/{reservationDetails.Id}");

        var cancelledMessage = Page.GetByTestId("cancel-reservation-geannuleerd");
        await Expect(cancelledMessage).ToBeVisibleAsync();
        await Expect(cancelledMessage).ToHaveTextAsync("Deze reservatie is geannuleerd. Je kan de details niet bekijken.");
    }

    [Test]
    public async Task ShowError_WrongCancel()
    {
        // Arrange:
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 2,
            Date = DateOnly.Parse(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd")),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 2,
            BoatPersonalName = "Swan",
            MentorName = "Jane Doe",
            BatteryType = "Nickel-Cadmium",
            IsDeleted = false
        };


        // Mock API responses
        await MockReservationDetailsApi(reservationDetails);
        await MockCancelReservationApi(reservationDetails.Id, status: 400);

        // Act: Navigate to reservation details page
        await NavigateToUrl($"/reservations/{reservationDetails.Id}");

        // Click the cancel button
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync(); // FIXME timeout error on this

        // Assert: Verify that the snackbar error message is displayed
        var errorMessage = Page.GetByTestId("cancel-reservation-error");
        await Expect(errorMessage).ToBeVisibleAsync();
        await Expect(errorMessage).ToHaveTextAsync($"Failed to cancel reservation with ID {reservationDetails.Id}. Response: Bad Request");
    }

    [Test]
    public async Task Admin_CancelsReservationSuccessfully_SameDay()
    {
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 10,
            Date = DateOnly.Parse(DateTime.Now.ToString("yyyy/MM/dd")), // Vandaag
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 10,
            BoatPersonalName = "Ocean Wave",
            MentorName = "Alice Smith",
            BatteryType = "Lithium-Ion",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);
        await MockCancelReservationApi(reservationDetails.Id);

        await LoginAsync(UserRole.Administrator); // Inloggen als Admin

        await NavigateToUrl($"/admin/reservations/{reservationDetails.Id}");
        var cancelButton = Page.GetByTestId("admin-reservation-cancel-button");
        await cancelButton.ClickAsync();

        await Expect(Page).ToHaveURLAsync("/admin/reservations");
    }
    [Test]
    public async Task Admin_PreventedFromCancelingPastReservation()
    {
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 11,
            Date = DateOnly.Parse(DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd")), // Gisteren
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 11,
            BoatPersonalName = "Sea Whisper",
            MentorName = "Bob Johnson",
            BatteryType = "Nickel-Metal Hydride",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);

        await LoginAsync(UserRole.Administrator); // Inloggen als Admin

        await NavigateToUrl($"/admin/reservations/{reservationDetails.Id}");
        var cancelButton = Page.GetByTestId("admin-reservation-cancel-button");
        await cancelButton.ClickAsync();

        var errorMessage = Page.GetByTestId("cancel-reservation-error");
        await Expect(errorMessage).ToBeVisibleAsync();
        await Expect(errorMessage).ToHaveTextAsync("Reservations in the past cannot be canceled.");
    }
    [Test]
    public async Task Admin_CannotCancelAlreadyCancelledReservation()
    {
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 12,
            Date = DateOnly.Parse(DateTime.Now.AddDays(5).ToString("yyyy/MM/dd")), // 5 dagen later
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 12,
            BoatPersonalName = "Blue Lagoon",
            MentorName = "Chris Thompson",
            BatteryType = "Lead-Acid",
            IsDeleted = true // Al geannuleerd
        };

        await MockReservationDetailsApi(reservationDetails);

        await LoginAsync(UserRole.Administrator); // Inloggen als Admin

        await NavigateToUrl($"/admin/reservations/{reservationDetails.Id}");
        var cancelButton = Page.GetByTestId("admin-reservation-cancel-button");
        await cancelButton.ClickAsync();

        var errorMessage = Page.GetByTestId("cancel-reservation-error");
        await Expect(errorMessage).ToBeVisibleAsync();
        await Expect(errorMessage).ToHaveTextAsync("The reservation is already canceled.");
    }
    [Test]
    public async Task Admin_CancelsReservationWithinTwoDays_Successfully()
    {
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 13,
            Date = DateOnly.Parse(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd")), // 1 dag later
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 13,
            BoatPersonalName = "Mystic River",
            MentorName = "David Parker",
            BatteryType = "Nickel-Cadmium",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);
        await MockCancelReservationApi(reservationDetails.Id);

        await LoginAsync(UserRole.Administrator); // Inloggen als Admin

        await NavigateToUrl($"/admin/reservations/{reservationDetails.Id}");
        var cancelButton = Page.GetByTestId("admin-reservation-cancel-button");
        await cancelButton.ClickAsync();

        await Expect(Page).ToHaveURLAsync("/admin/reservations");
    }
    [Test]
    public async Task Admin_PreventedFromCancelingInvalidReservation()
    {
        var nonExistentReservationId = 9999;

        await MockCancelReservationApi(nonExistentReservationId, status: 404);

        await LoginAsync(UserRole.Administrator); // Inloggen als Admin

        await NavigateToUrl($"/admin/reservations/{nonExistentReservationId}");

        var cancelButton = Page.GetByTestId("admin-reservation-cancel-button");
        await cancelButton.ClickAsync();

        var errorMessage = Page.GetByTestId("cancel-reservation-error");
        await Expect(errorMessage).ToBeVisibleAsync();
        await Expect(errorMessage).ToHaveTextAsync("Reservation not found.");
    }








}

