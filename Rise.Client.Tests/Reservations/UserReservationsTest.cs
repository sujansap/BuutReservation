using Microsoft.Playwright;
using Rise.Shared.Reservations;
using Rise.Shared.Pagination;

namespace Rise.Client.Tests.Reservations
{

    [TestFixture]
    public class UserReservationsTest : CustomPageTest
    {

        private const string UserReservationsUrl = "/reservations?CurrentTab=reservations";

        private readonly ReservationDto ValidReservation = new()
        {
            BoatId = 1,
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatPersonalName = "Limba"
        };

        private readonly ReservationDto PastReservation = new()
        {
            BoatId = 2,
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
            Start = TimeOnly.Parse("09:00"),
            End = TimeOnly.Parse("12:00"),
            BoatPersonalName = "Speedy"
        };

        private async Task MockReservationsApi()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
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
        private async Task MockPastReservationsApi()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                await Task.Delay(3000);

                var response = new ItemsPageDto<ReservationDto>()
                {
                    Data = [PastReservation],
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
            await InitNavigationToUrl(UserReservationsUrl);
            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();
        }


        [Test]
        public async Task DoesNotHaveLegendComponent()
        {
            await InitNavigationToUrl(UserReservationsUrl);
            ILocator legend = Page.GetByTestId("custom-calendar-legend");
            await Expect(legend).ToHaveCountAsync(0);
        }


        [Test]
        public async Task HasCorrectAmountOfReservations()
        {
            await MockReservationsApi();
            await InitNavigationToUrl(UserReservationsUrl);



            ILocator locator = Page.GetByTestId("reservation-item");
            await Expect(locator).ToHaveCountAsync(1);
        }

        [Test]
        public async Task ShowsReservations()
        {
            await MockReservationsApi();
            await InitNavigationToUrl(UserReservationsUrl);



            ILocator firstReservation = Page.GetByTestId("reservation-item").First;

            await Expect(firstReservation.GetByTestId("reservation-date")).ToContainTextAsync(ValidReservation.Date.ToString("dd/MM/yyyy"));
            await Expect(firstReservation.GetByTestId("reservation-boat-name")).ToContainTextAsync(ValidReservation.BoatPersonalName);
            await Expect(firstReservation.GetByTestId("reservation-time")).ToContainTextAsync($"{ValidReservation.Start:HH:mm} - {ValidReservation.End:HH:mm}");

        }


        [Test]
        public async Task ShowsLoadingStateWhileFetchingReservations()
        {
            await MockReservationsApi();

            await InitNavigationToUrl(UserReservationsUrl);




            await Expect(Page.GetByTestId("user-reservations-loading-progress")).ToBeVisibleAsync(new() { Timeout = 8000 });

            await Page.WaitForSelectorAsync("[data-testid='user-reservations-loading-progress']");
        }

        [Test]
        public async Task ShowsEmptyStateWhenNoReservations()
        {
            await MockEmptyReservationsApi();
            await InitNavigationToUrl(UserReservationsUrl);

            // Wait for loading to complete AND for the empty state message to appear
            await Page.WaitForSelectorAsync("[data-testid='user-reservations-loading-progress']", new() { State = WaitForSelectorState.Hidden, Timeout = 10000 });
            await Page.WaitForSelectorAsync("[data-testid='no-reservations']", new() { State = WaitForSelectorState.Visible, Timeout = 10000 });

            ILocator emptyStateMessage = Page.GetByTestId("no-reservations");
            await Expect(emptyStateMessage).ToBeVisibleAsync();
            await Expect(emptyStateMessage).ToHaveTextAsync("U heeft geen aankomende reserveringen.");

            await Expect(Page.GetByTestId("reservation-item")).ToHaveCountAsync(0);
        }

        [Test]
        public async Task ShowsErrorStateWhenApiReturns400()
        {
            await MockReservationsApiError();
            await InitNavigationToUrl(UserReservationsUrl);

            // Wait for loading to complete
            await Page.WaitForSelectorAsync("[data-testid='user-reservations-loading-progress']", new() { State = WaitForSelectorState.Hidden });
            await Page.WaitForSelectorAsync("[data-testid='user-reservations-fetch-error']", new() { State = WaitForSelectorState.Visible });

            ILocator errorMessage = Page.GetByTestId("user-reservations-fetch-error");
            await Expect(errorMessage).ToBeVisibleAsync();

            await Expect(Page.GetByTestId("reservation-item")).ToHaveCountAsync(0);
        }

        // TODO: test amount of reservations on past pages
        // TODO: Show reservations
        // TODO: test toggling past reservations

        [Test]
        public async Task ShowsPastReservations()
        {
            await MockPastReservationsApi();
            await InitNavigationToUrl(UserReservationsUrl + "&Past=true");

            ILocator locator = Page.GetByTestId("reservation-item");
            await Expect(locator).ToHaveCountAsync(1);
        }

        [Test]
        public async Task HasPastTab()
        {
            // test if past tab is present
            await InitNavigationToUrl(UserReservationsUrl);
            await Page.GetByTestId("tab-past-reservations").IsVisibleAsync();
        }


        // now add a test that checks the info in past reservations
        [Test]
        public async Task CheckPastReservations()
        {
            await MockPastReservationsApi();
            await InitNavigationToUrl(UserReservationsUrl + "&Past=true");

            ILocator firstReservation = Page.GetByTestId("reservation-item").First;

            await Expect(firstReservation.GetByTestId("reservation-date")).ToContainTextAsync(PastReservation.Date.ToString("dd/MM/yyyy"));
            await Expect(firstReservation.GetByTestId("reservation-boat-name")).ToContainTextAsync(PastReservation.BoatPersonalName);
            await Expect(firstReservation.GetByTestId("reservation-time")).ToContainTextAsync($"{PastReservation.Start:HH:mm} - {PastReservation.End:HH:mm}");

        }
    }

}

