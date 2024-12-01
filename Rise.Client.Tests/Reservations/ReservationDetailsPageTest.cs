// using System.Text.Json;
// using System.Text.RegularExpressions;
// using Rise.Shared.Reservations;

// namespace Rise.Client.Tests.Reservations
// {
//     [TestFixture]
//     public class ReservationDetailsPageTest : CustomPageTest
//     {
//         // TODO: auth all reservation urls
//         private const string UserReservationDetailsUrl = "/reservations/1";
//         private const string InvalidReservationDetailsUrl = "/reservations/100";


//         private async Task MockReservationDetailsApi(ReservationDetailsDto? response = null, int status = 200)
//         {
//             await Page.RouteAsync(new Regex("^.*/api/Reservation/\\d+"), async route =>
//             {
//                 await route.FulfillAsync(new()
//                 {
//                     Status = status,
//                     ContentType = "application/json",
//                     Body = response != null ? JsonSerializer.Serialize(response) : string.Empty
//                 });
//             });
//         }

//         [Test]
//         public async Task ShowsReservationDetails()
//         {
//             ReservationDetailsDto reservationDetails = new()
//             {
//                 Id = 1,
//                 Date = new DateOnly(2024, 10, 30),
//                 Start = new TimeOnly(10, 0, 0),
//                 End = new TimeOnly(13, 0, 0),
//                 BoatId = 101,
//                 BoatPersonalName = "Limba",
//                 MentorName = "John Doe",
//                 BatteryType = "Lithium-Ion"
//             };

//             await MockReservationDetailsApi(reservationDetails);
//             await NavigateToUrl(UserReservationDetailsUrl);

//             await Expect(Page.GetByTestId("reservation-date")).ToContainTextAsync(reservationDetails.Date.ToString("dd/MM/yyyy"));
//             await Expect(Page.GetByTestId("reservation-boat")).ToContainTextAsync(reservationDetails.BoatPersonalName);
//             await Expect(Page.GetByTestId("reservation-time")).ToContainTextAsync($"{reservationDetails.Start:HH:mm} - {reservationDetails.End:HH:mm}");
//             await Expect(Page.GetByTestId("reservation-battery")).ToContainTextAsync(reservationDetails.BatteryType);
//             await Expect(Page.GetByTestId("reservation-battery-mentor")).ToContainTextAsync(reservationDetails.MentorName);
//         }

//         [Test]
//         public async Task ShowsNotFoundErrorForNonExistentReservation()
//         {
//             await MockReservationDetailsApi(null, status: 404);
//             await NavigateToUrl(UserReservationDetailsUrl);

//             var errorMessage = Page.Locator("text='Response status code does not indicate success: 404 (Not Found).'");

//             await Expect(errorMessage).ToBeVisibleAsync(new() { Timeout = 30000 });
//         }

// [Test]
// public async Task ShowsCurrentBatteryUserWhenAvailable()
// {
//     var reservationDetails = new ReservationDetailsDto
//     {
//         Id = 1,
//         Date = DateOnly.Parse("2024/10/30"),
//         Start = TimeOnly.Parse("10:00"),
//         End = TimeOnly.Parse("13:00"),
//         BoatId = 101,
//         BoatPersonalName = "Limba",
//         MentorName = "John Doe",
//         BatteryType = "Lithium-Ion",
//         CurrentBatteryUserId = 42,
//         CurrentBatteryUserName = "Jane Smith"
//     };

//     await MockReservationDetailsApi(reservationDetails);
//     await Page.GotoAsync(UserReservationDetailsUrl);

//     await Page.WaitForRequestAsync(request => request.Url.Contains("api/Reservation/1"));

//     var currentUserText = await Page.GetByTestId("reservation-battery-current-user").TextContentAsync();
//     currentUserText.ShouldContain(reservationDetails.CurrentBatteryUserName);
// }

// [Test]
// public async Task HidesCurrentBatteryUserWhenNotAvailable()
// {
//     var reservationDetails = new ReservationDetailsDto
//     {
//         Id = 1,
//         Date = DateOnly.Parse("2024/10/30"),
//         Start = TimeOnly.Parse("10:00"),
//         End = TimeOnly.Parse("13:00"),
//         BoatId = 101,
//         BoatPersonalName = "Limba",
//         MentorName = "John Doe",
//         BatteryType = "Lithium-Ion",
//         CurrentBatteryUserId = null,
//         CurrentBatteryUserName = null
//     };

//     await MockReservationDetailsApi(reservationDetails);
//     await Page.GotoAsync(UserReservationDetailsUrl);

//     await Page.WaitForRequestAsync(request => request.Url.Contains("api/Reservation/1"));

//     var currentUserElement = Page.GetByTestId("reservation-battery-current-user");
//     await Expect(currentUserElement).ToHaveCountAsync(0);
// }




//     }
// }