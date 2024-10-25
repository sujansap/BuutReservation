// TODO: Add tests for UserReservations.razor
using System;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Shouldly;
using System.Threading.Tasks;
using Rise.Shared.Reservations;

namespace Rise.Client.Tests.Reservations {

    [TestClass]
    public class UserReservationsTest : PageTest
    {
        private const string UserReservationsUrl = "https://localhost:5001/reservations/your-reservations";

        private ReservationListDto ValidReservation = new (){
            Id = 1,
            Date = DateOnly.Parse("2024-03-15"),
            Start = TimeOnly.Parse("09:00"),
            End = TimeOnly.Parse("11:00"),
            BoatPersonalName = "Sailboat 1"
        };

        private async Task MockReservationsApi()
        {
            await Page.RouteAsync("*/**/api/reservations/user/**", async route =>
            {
                var json = new object[]
                {
                    new { id = ValidReservation.Id, date = ValidReservation.Date.ToString("yyyy-MM-dd"), startTime = ValidReservation.Start.ToString("HH:mm"), endTime = ValidReservation.End.ToString("HH:mm"), boatName = ValidReservation.BoatPersonalName },
                    new { id = 2, date = "2024-03-16", startTime = "14:00", endTime = "16:00", boatName = "Motorboat 1" },
                    new { id = 3, date = "2024-03-17", startTime = "10:00", endTime = "12:00", boatName = "Kayak 1" },
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

            var reservations = await Page.GetByTestId("reservation").AllAsync();
            reservations.Count.ShouldBe(3);
        }

        [TestMethod]
        public async Task ShowsReservations()
        {
            await MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            var firstReservation = Page.GetByTestId("reservation").First;
            (await firstReservation.GetByTestId("boatname").InnerTextAsync()).ShouldBe(ValidReservation.BoatPersonalName);
            (await firstReservation.GetByTestId("date").InnerTextAsync()).ShouldBe(ValidReservation.Date.ToString("yyyy-MM-dd"));
            (await firstReservation.GetByTestId("time").InnerTextAsync()).ShouldBe($"{ValidReservation.Start.ToString("HH:mm")} - {ValidReservation.End.ToString("HH:mm")}");
            
        }
     
        
        [TestMethod]
        public async Task ShowsLoadingStateWhileFetchingReservations()
        {
            var mockTask = MockReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            (await Page.GetByTestId("loading-indicator").IsVisibleAsync()).ShouldBeTrue();
            await mockTask;

            await Page.WaitForSelectorAsync("[data-testid='loading-indicator']", new() { State = WaitForSelectorState.Hidden });

        }

        [TestMethod]
        public async Task ShowsEmptyStateWhenNoReservations()
        {
            await MockEmptyReservationsApi();
            await Page.GotoAsync(UserReservationsUrl);

            await Page.WaitForSelectorAsync("[data-testid='loading-indicator']", new() { State = WaitForSelectorState.Hidden });

            var emptyStateMessage = Page.GetByTestId("empty-state-message");
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

            await Page.WaitForSelectorAsync("[data-testid='loading-indicator']", new() { State = WaitForSelectorState.Hidden });

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

