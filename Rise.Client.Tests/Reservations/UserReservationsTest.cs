using Microsoft.Playwright;
using Shouldly;
using Rise.Shared.Reservations;

namespace Rise.Client.Reservations {

    [TestFixture]
    public class UserReservationsTest : CustomPageTest
    {

        private const string UserReservationsUrl = "/reservations/user-reservation";

        private ReservationDto ValidReservation = new (){
            Id = 1,
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
            // TODO: add delay to the response after 5 seconds
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                var json = new object[]
                {
                    new { 
                        id = ValidReservation.Id, 
                        date = ValidReservation.Date.ToString("dd/MM/yyyy"),
                        startTime = ValidReservation.Start.ToString("HH:mm"),
                        endTime = ValidReservation.End.ToString("HH:mm"),
                        boatPersonalName = ValidReservation.BoatPersonalName
                    },
                    new { id = 2, date = "16/03/2024", startTime = "14:00", endTime = "16:00", boatPersonalName = "Motorboat 1" },
                    new { id = 3, date = "17/03/2024", startTime = "10:00", endTime = "12:00", boatPersonalName = "Kayak 1" },
                };
                await route.FulfillAsync(new() { Json = json });
            });
        }

        private async Task MockEmptyReservationsApi()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                await route.FulfillAsync(new() { Json = new object[] { } });
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
            
            // Wait for loading to complete
            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });
            
            var locator = Page.GetByTestId("reservation");
            await Expect(locator).ToHaveCountAsync(3, new LocatorAssertionsToHaveCountOptions() { Timeout = 8000 });
        }

        [Test]
        public async Task ShowsReservations()
        {
            await MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            var firstReservation = Page.GetByTestId("reservation").First;

            (await firstReservation.GetByTestId("reservation-date").InnerTextAsync()).ShouldContain(ValidReservation.Date.ToString("dd/MM/yyyy"));
            (await firstReservation.GetByTestId("reservation-boat-name").InnerTextAsync()).ShouldContain(ValidReservation.BoatPersonalName);
            (await firstReservation.GetByTestId("reservation-time").InnerTextAsync()).ShouldContain($"{ValidReservation.Start.ToString("HH:mm")} - {ValidReservation.End.ToString("HH:mm")}");
            
        }


        [Test]
        public async Task ShowsLoadingStateWhileFetchingReservations()
        {
            var mockTask = MockReservationsApi();
            
            await Page.GotoAsync(UserReservationsUrl);
            (await Page.GetByTestId("loading-progress").IsVisibleAsync()).ShouldBeTrue();
            
            await mockTask;
            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });
        }

        [Test]
        public async Task ShowsEmptyStateWhenNoReservations()
        {
            await MockEmptyReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });

            var emptyStateMessage = Page.GetByTestId("no-reservations");
            (await emptyStateMessage.IsVisibleAsync()).ShouldBeTrue();
            (await emptyStateMessage.TextContentAsync()).ShouldBe("You have no reservations.");

            var reservations = await Page.GetByTestId("reservation").AllAsync();
            reservations.Count.ShouldBe(0);
        }

        [Test]
        public async Task ShowsErrorStateWhenApiReturns400()
        {
            await MockReservationsApiError();
            await Page.GotoAsync(UserReservationsUrl);

            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });

            var errorMessage = Page.GetByTestId("error-message");
            (await errorMessage.IsVisibleAsync()).ShouldBeTrue();
            (await errorMessage.TextContentAsync() ?? "").ShouldContain("An error occurred while fetching your reservations");

            var reservations = await Page.GetByTestId("reservation").AllAsync();
            reservations.Count.ShouldBe(0);
        }
    }
}

// todo: test for error state 400

