using Microsoft.Playwright;
using Rise.Shared.Reservations;
using Rise.Shared.Pagination;

namespace Rise.Client.Tests.Reservations
{

    [TestFixture]
    public class UserReservationsTest : CustomAuthenticatedPageTest
    {

        private const string UserReservationsUrl = "/reservations?CurrentTab=reservations";

        private readonly ReservationDto ValidReservation = new()
        {
            BoatId = 1,
            Date = DateOnly.Parse("2024-10-30"),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatPersonalName = "Limba"
        };

        private async Task MockReservationsApi()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                // Reduce delay from 10000ms to 3000ms
                await Task.Delay(3000);
                var response = new ItemsPageDto<ReservationDto>()
                {
                    Data = [ValidReservation],
                    NextId = 1,
                    PreviousId = 1,
                    IsFirstPage = true
                };
                await route.FulfillAsync(new()
                {
                    ContentType = "application/json",
                    Body = System.Text.Json.JsonSerializer.Serialize(response)
                });
            });
        }

        private async Task MockEmptyReservationsApi()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                await Task.Delay(3000);
                var response = new ItemsPageDto<ReservationDto>()
                {
                    Data = [],
                    NextId = null,
                    PreviousId = null,
                    IsFirstPage = true
                };
                await route.FulfillAsync(new()
                {
                    ContentType = "application/json",
                    Body = System.Text.Json.JsonSerializer.Serialize(response)
                });
            });
        }

        private async Task MockReservationsApiError()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                await route.FulfillAsync(new() { Status = 400, Body = "Bad Request" });
            });
        }

        [Test]
        public async Task HasTabs()
        {
            await Page.GotoAsync(UserReservationsUrl);
            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();
        }


        [Test]
        public async Task DoesNotHaveLegendComponent()
        {
            await Page.GotoAsync(UserReservationsUrl);
            ILocator legend = Page.GetByTestId("custom-calendar-legend");
            await Expect(legend).ToHaveCountAsync(0);
        }


        [Test]
        public async Task HasCorrectAmountOfReservations()
        {
            await MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            await Page.WaitForRequestAsync(request => request.Url.Contains("api/Reservation/me"));

            ILocator locator = Page.GetByTestId("reservation-item");
            await Expect(locator).ToHaveCountAsync(1);
        }

        [Test]
        public async Task ShowsReservations()
        {
            await MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            await Page.WaitForRequestAsync(request => request.Url.Contains("api/Reservation/me"));

            ILocator firstReservation = Page.GetByTestId("reservation-item").First;

            await Expect(firstReservation.GetByTestId("reservation-date")).ToContainTextAsync(ValidReservation.Date.ToString("dd/MM/yyyy"));
            await Expect(firstReservation.GetByTestId("reservation-boat-name")).ToContainTextAsync(ValidReservation.BoatPersonalName);
            await Expect(firstReservation.GetByTestId("reservation-time")).ToContainTextAsync($"{ValidReservation.Start:HH:mm} - {ValidReservation.End:HH:mm}");

        }


        [Test]
        public async Task ShowsLoadingStateWhileFetchingReservations()
        {
            await MockReservationsApi();

            await Page.GotoAsync(UserReservationsUrl);

            await Page.WaitForRequestAsync(request => request.Url.Contains("api/Reservation/me"));


            await Expect(Page.GetByTestId("user-reservations-loading-progress")).ToBeVisibleAsync(new() { Timeout = 8000 });

            await Page.WaitForSelectorAsync("[data-testid='user-reservations-loading-progress']");
        }

        [Test]
        public async Task ShowsEmptyStateWhenNoReservations()
        {
            await MockEmptyReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            // Wait for loading to complete AND for the empty state message to appear
            await Page.WaitForSelectorAsync("[data-testid='user-reservations-loading-progress']", new() { State = WaitForSelectorState.Hidden });
            await Page.WaitForSelectorAsync("[data-testid='no-reservations']", new() { State = WaitForSelectorState.Visible });

            ILocator emptyStateMessage = Page.GetByTestId("no-reservations");
            await Expect(emptyStateMessage).ToBeVisibleAsync();
            await Expect(emptyStateMessage).ToHaveTextAsync("Geen reservaties gevonden.");

            await Expect(Page.GetByTestId("reservation-item")).ToHaveCountAsync(0);
        }

        [Test]
        public async Task ShowsErrorStateWhenApiReturns400()
        {
            await MockReservationsApiError();
            await Page.GotoAsync(UserReservationsUrl);

            // Wait for loading to complete
            await Page.WaitForSelectorAsync("[data-testid='user-reservations-loading-progress']", new() { State = WaitForSelectorState.Hidden });
            await Page.WaitForSelectorAsync("[data-testid='user-reservations-fetch-error']", new() { State = WaitForSelectorState.Visible });

            ILocator errorMessage = Page.GetByTestId("user-reservations-fetch-error");
            await Expect(errorMessage).ToBeVisibleAsync();

            await Expect(Page.GetByTestId("reservation-item")).ToHaveCountAsync(0);
        }
    }
}

