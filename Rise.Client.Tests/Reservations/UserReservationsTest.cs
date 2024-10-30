// TODO: Add tests for UserReservations.razor
using System;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Shouldly;
using System.Threading.Tasks;
using Rise.Shared.Reservations;
using System.IO;

namespace Rise.Client.Reservations {

    [TestClass]
    public class UserReservationsTest : PageTest
    {

            [TestInitialize]
    public async Task TestInitialize()
    {
         await Context.Tracing.StartAsync(new()
        {
            Title = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        await Context.Tracing.StopAsync(new()
        {
            Path = Path.Combine(
                Environment.CurrentDirectory,
                "playwright-traces",
                $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}.zip"
            )
        });
    }
    
        private const string UserReservationsUrl = "https://localhost:5001/reservations/your-reservations";

        private ReservationDto ValidReservation = new (){
            Id = 1,
            Date = DateOnly.Parse("2024-10-30"),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatPersonalName = "Limba"
        };

        private async Task MockReservationsApi()
        {
            // TODO: add delay to the response after 5 seconds
            await Page.RouteAsync("*/**/api/reservations/user/**", async route =>
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
            await Page.RouteAsync("*/**/api/reservations/user/**", async route =>
            {
                await route.FulfillAsync(new() { Json = new object[] { } });
            });
        }

        private async Task MockReservationsApiError()
        {
            await Page.RouteAsync("*/**/api/reservations/user/**", async route =>
            {
                await route.FulfillAsync(new() { Status = 400, Body = "Bad Request" });
            });
        }

        [TestMethod]
        public async Task HasTabs()
        {
            await Page.GotoAsync(UserReservationsUrl);
            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();
        }

        
        [TestMethod]
        public async Task DoesNotHaveLegendComponent()
        {
            await Page.GotoAsync(UserReservationsUrl);
            var legend = Page.GetByTestId("custom-calendar-legend");
            (await legend.CountAsync()).ShouldBe(0);
        }


        [TestMethod]
        public async Task HasCorrectAmountOfReservations()
        {
            await MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);
            
            // Wait for loading to complete
            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });
            
            var locator = Page.GetByTestId("reservation");
            await Expect(locator).ToHaveCountAsync(3);
        }

        [TestMethod]
        public async Task ShowsReservations()
        {
            await MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            var firstReservation = Page.GetByTestId("reservation").First;

            (await firstReservation.GetByTestId("reservation-date").InnerTextAsync()).ShouldContain(ValidReservation.Date.ToString("dd/MM/yyyy"));
            (await firstReservation.GetByTestId("reservation-boat-name").InnerTextAsync()).ShouldContain(ValidReservation.BoatPersonalName);
            (await firstReservation.GetByTestId("reservation-time").InnerTextAsync()).ShouldContain($"{ValidReservation.Start.ToString("HH:mm")} - {ValidReservation.End.ToString("HH:mm")}");
            
        }
     
        
        [TestMethod]
        public async Task ShowsLoadingStateWhileFetchingReservations()
        {
            var mockTask = MockReservationsApi();
            
            await Page.GotoAsync(UserReservationsUrl);
            (await Page.GetByTestId("loading-progress").IsVisibleAsync()).ShouldBeTrue();
            
            await mockTask;
            await Page.WaitForSelectorAsync("[data-testid='loading-progress']", new() { State = WaitForSelectorState.Hidden });
        }

        [TestMethod]
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

        [TestMethod]
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


// todo: test loading state DONE
// todo: test data that comes in DONE
// todo: test for empty list of data DONE
// todo: test for error state 400

