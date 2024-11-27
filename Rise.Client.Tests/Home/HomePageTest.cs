using System.Text.RegularExpressions;

namespace Rise.Client.Tests.Home
{
    [TestFixture]
    public class HomePageTest : CustomPageTest
    {

        [Test]
        public async Task HasHeroImage()
        {
            //Arrange
            await NavigateToUrl("/home");

            //Act
            var heroImage = Page.GetByTestId("hero-image");

            //Assert
            await Expect(heroImage).ToBeVisibleAsync();
        }

        [Test]
        public async Task HasHeroTitle()
        {
            //Arrange
            await NavigateToUrl("/home");

            //Act
            var heroTitle = Page.GetByTestId("hero-title");

            //Assert
            await Expect(heroTitle).ToBeVisibleAsync();
        }

        [Test]
        public async Task HasReservationButton()
        {
            //Arrange
            await NavigateToUrl("/home");

            //Act
            var reservationButton = Page.GetByTestId("reservation-button");

            //Assert
            await Expect(reservationButton).ToBeVisibleAsync();
        }

        [Test]
        public async Task ClickReservationButton_ShouldNavigateToReservations()
        {
            //Arrange
            await NavigateToUrl("/home");

            //Act
            var reservationButton = Page.GetByTestId("reservation-button");
            await reservationButton.ClickAsync();

            //Assert
            await Expect(Page).ToHaveURLAsync(new Regex("/reservations\\?.*$"));
        }

        [Test]
        public async Task HasBoatsSection()
        {
            //Arrange
            await NavigateToUrl("/home");

            //Act
            var boatsSection = Page.GetByTestId("boats-section");

            //Assert
            await Expect(boatsSection).ToBeVisibleAsync();
        }
    }
}