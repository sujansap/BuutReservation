using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Shouldly;

namespace Rise.Client.Tests.Home
{
    [TestFixture]
    public class HomePageTest : CustomPageTest
    {

        [Test]
        public async Task HasHeroImage()
        {
            //Arrange
            await InitNavigationToUrl("/home");

            //Act
            var heroImage = Page.GetByTestId("hero-image");

            //Assert
            await Expect(heroImage).ToBeVisibleAsync();
        }

        [Test]
        public async Task HasHeroTitle()
        {
            //Arrange
            await InitNavigationToUrl("/home");

            //Act
            var heroTitle = Page.GetByTestId("hero-title");

            //Assert
            await Expect(heroTitle).ToBeVisibleAsync();
        }

        [Test]
        public async Task HasReservationButton()
        {
            //Arrange
            await InitNavigationToUrl("/home");

            //Act
            var reservationButton = Page.GetByTestId("reservation-button");

            //Assert
            await Expect(reservationButton).ToBeVisibleAsync();
        }

        [Test]
        public async Task ClickReservationButton_ShouldNavigateToReservations()
        {
            //Arrange
            await InitNavigationToUrl("/home");

            //Act
            var reservationButton = Page.GetByTestId("reservation-button");
            await reservationButton.ClickAsync();

            //Assert
            var url = Page.Url;
            var baseUrl = string.Join("/", url.Split('/').Take(3));
            url.ShouldContain($"{baseUrl}/reservations");
        }

        [Test]
        public async Task HasBoatsSection()
        {
            //Arrange
            await InitNavigationToUrl("/home");

            //Act
            var boatsSection = Page.GetByTestId("boats-section");

            //Assert
            await Expect(boatsSection).ToBeVisibleAsync();
        }
    }
}