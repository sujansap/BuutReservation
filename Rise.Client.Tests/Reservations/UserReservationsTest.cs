using Microsoft.Playwright;
using Shouldly;
using Rise.Shared.Reservations;
using Rise.Shared.Pagination;

namespace Rise.Client.Reservations {

    [TestFixture]
    public class UserReservationsTest : CustomPageTest
    {

        private const string UserReservationsUrl = "/reservations?CurrentTab=reservations";

        private ReservationDto ValidReservation = new (){
            BoatId = 1,
            Date = DateOnly.Parse("2024-10-30"),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatPersonalName = "Limba"
        };

        [SetUp]
        public async Task Setup()
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }

        [TearDown]
        public async Task TearDown()
        {
            await Context.Tracing.StopAsync(new()
            {
                Path = Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    "playwright-traces",
                    $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
                )
            });
        }

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
                await route.FulfillAsync(new() { 
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
                await route.FulfillAsync(new() { 
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
            var legend = Page.GetByTestId("custom-calendar-legend");
            (await legend.CountAsync()).ShouldBe(0);
        }


        [Test]
        public async Task HasCorrectAmountOfReservations()
        {
            await MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);
            
            // Wait for loading to complete AND for at least one reservation to appear
            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });
            await Page.WaitForSelectorAsync("[data-testid='reservation-item']", new() { State = WaitForSelectorState.Visible });
            
            var locator = Page.GetByTestId("reservation-item");
            await Expect(locator).ToHaveCountAsync(1);
        }

        [Test]
        public async Task ShowsReservations()
        {
            await MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            var firstReservation = Page.GetByTestId("reservation-item").First;

            (await firstReservation.GetByTestId("reservation-date").InnerTextAsync()).ShouldContain(ValidReservation.Date.ToString("dd/MM/yyyy"));
            (await firstReservation.GetByTestId("reservation-boat-name").InnerTextAsync()).ShouldContain(ValidReservation.BoatPersonalName);
            (await firstReservation.GetByTestId("reservation-time").InnerTextAsync()).ShouldContain($"{ValidReservation.Start.ToString("HH:mm")} - {ValidReservation.End.ToString("HH:mm")}");
            
        }


        [Test]
        public async Task ShowsLoadingStateWhileFetchingReservations()
        {
            var mockTask = MockReservationsApi();
            
            await Page.GotoAsync(UserReservationsUrl);
            await Expect(Page.GetByTestId("loading-progress")).ToBeVisibleAsync();
            
            await mockTask;
            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });
        }

        [Test]
        public async Task ShowsEmptyStateWhenNoReservations()
        {
            await MockEmptyReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            // Wait for loading to complete AND for the empty state message to appear
            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });
            await Page.WaitForSelectorAsync("[data-testid='no-reservations']", new() { State = WaitForSelectorState.Visible });

            var emptyStateMessage = Page.GetByTestId("no-reservations");
            (await emptyStateMessage.IsVisibleAsync()).ShouldBeTrue();
            (await emptyStateMessage.TextContentAsync()).ShouldBe("Geen reservaties gevonden.");

            var reservations = await Page.GetByTestId("reservation-item").AllAsync();
            reservations.Count.ShouldBe(0);
        }

        [Test]
        public async Task ShowsErrorStateWhenApiReturns400()
        {
            await MockReservationsApiError();
            await Page.GotoAsync(UserReservationsUrl);

            // Wait for loading to complete
            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });
            await Page.WaitForSelectorAsync("[data-testid='error-message']", new() { State = WaitForSelectorState.Visible });

            var errorMessage = Page.GetByTestId("error-message");
            (await errorMessage.IsVisibleAsync()).ShouldBeTrue();
            (await errorMessage.TextContentAsync() ?? "").ShouldContain("ErrorFetchingReservations");

            var reservations = await Page.GetByTestId("reservation-item").AllAsync();
            reservations.Count.ShouldBe(0);
        }
    }
}

