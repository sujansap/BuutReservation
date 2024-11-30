using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Rise.Client.Tests.Register
{
    [TestFixture]
    public class RegisterFormPageTest : CustomPageTest
    {
        [Test]
        public async Task ShouldLoadSuccessfully()
        {
            await Page.GotoAsync("/register");

            var title = Page.GetByTestId("page-title");
            var container = Page.GetByTestId("register-container");
            var paper = Page.GetByTestId("register-paper");
            var form = Page.GetByTestId("register-form");

            await Expect(title).Not.ToBeEmptyAsync();
            await Expect(container).ToBeVisibleAsync();
            await Expect(paper).ToBeVisibleAsync();
            await Expect(form).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldInitializeFormWithEmptyValuesAndNotEmptyCountry()
        {
            await Page.GotoAsync("/register");

            var emailField = Page.GetByTestId("email-field");
            var passwordField = Page.GetByTestId("password-field");
            var repeatPasswordField = Page.GetByTestId("repeat-password-field");
            var firstNameField = Page.GetByTestId("first-name-field");
            var lastNameField = Page.GetByTestId("last-name-field");
            var dateOfBirthField = Page.GetByTestId("date-of-birth-picker");
            var phoneNumberField = Page.GetByTestId("phone-number-field");

            var streetField = Page.GetByTestId("street-field");
            var houseNumberField = Page.GetByTestId("house-number-field");
            var cityField = Page.GetByTestId("city-field");
            var postalCodeField = Page.GetByTestId("postal-code-field");
            var countryField = Page.GetByTestId("country-field");

            await Expect(emailField).ToBeEditableAsync();
            await Expect(emailField).ToBeEmptyAsync();

            await Expect(passwordField).ToBeEditableAsync();
            await Expect(passwordField).ToBeEmptyAsync();

            await Expect(repeatPasswordField).ToBeEditableAsync();
            await Expect(repeatPasswordField).ToBeEmptyAsync();

            await Expect(firstNameField).ToBeEditableAsync();
            await Expect(firstNameField).ToBeEmptyAsync();

            await Expect(lastNameField).ToBeEditableAsync();
            await Expect(lastNameField).ToBeEmptyAsync();

            await Expect(dateOfBirthField).ToBeEditableAsync();
            await Expect(dateOfBirthField).ToBeEmptyAsync();

            await Expect(phoneNumberField).ToBeEditableAsync();
            await Expect(phoneNumberField).ToBeEmptyAsync();

            await Expect(streetField).ToBeEditableAsync();
            await Expect(streetField).ToBeEmptyAsync();

            await Expect(houseNumberField).ToBeEditableAsync();
            await Expect(houseNumberField).ToBeEmptyAsync();

            await Expect(cityField).ToBeEditableAsync();
            await Expect(cityField).ToBeEmptyAsync();

            await Expect(postalCodeField).ToBeEditableAsync();
            await Expect(postalCodeField).ToBeEmptyAsync();

            await Expect(countryField).Not.ToBeEditableAsync();
            await Expect(countryField).Not.ToBeEmptyAsync();

        }

        [Test]
        public async Task ShouldLoadRegisterButtonDisabledAndResetButtonEnabled()
        {
            await Page.GotoAsync("/register");

            var registerButton = Page.GetByTestId("register-button");
            var resetButton = Page.GetByTestId("reset-button");

            await Expect(registerButton).ToBeDisabledAsync();
            await Expect(resetButton).ToBeEnabledAsync();
        }

        [Test]
        public async Task ShouldValidateAndSubmitSuccessfullyAndResetToDefault()
        {
            await Page.GotoAsync("/register");

            var emailField = Page.GetByTestId("email-field");
            await emailField.FillAsync("Test@test.com");
            var passwordField = Page.GetByTestId("password-field");
            await passwordField.FillAsync("12345678");
            var repeatPasswordField = Page.GetByTestId("repeat-password-field");
            await repeatPasswordField.FillAsync("12345678");
            var firstNameField = Page.GetByTestId("first-name-field");
            await firstNameField.FillAsync("test");
            var lastNameField = Page.GetByTestId("last-name-field");
            await lastNameField.FillAsync("test");
            var dateOfBirthField = Page.GetByTestId("date-of-birth-picker");
            await dateOfBirthField.FillAsync("2005/01/01");
            var phoneNumberField = Page.GetByTestId("phone-number-field");
            await phoneNumberField.FillAsync("003212345678");

            var streetField = Page.GetByTestId("street-field");
            await streetField.FillAsync("teststraat");
            var houseNumberField = Page.GetByTestId("house-number-field");
            await houseNumberField.FillAsync("45");
            var cityField = Page.GetByTestId("city-field");
            await cityField.FillAsync("Gent");
            var postalCodeField = Page.GetByTestId("postal-code-field");
            await postalCodeField.FillAsync("9000");
            var countryField = Page.GetByTestId("country-field");

            var submitButton = Page.GetByTestId("register-button");
            var resetButton = Page.GetByTestId("reset-button");

            await Expect(submitButton).ToBeEnabledAsync();
            await Expect(resetButton).ToBeEnabledAsync();

            await submitButton.ClickAsync();
            await Expect(Page.Locator("div.mud-snackbar.mud-snackbar-success")).ToBeVisibleAsync();

            await Expect(emailField).ToBeEmptyAsync();
            await Expect(passwordField).ToBeEmptyAsync();
            await Expect(repeatPasswordField).ToBeEmptyAsync();
            await Expect(firstNameField).ToBeEmptyAsync();
            await Expect(lastNameField).ToBeEmptyAsync();
            await Expect(dateOfBirthField).ToBeEmptyAsync();
            await Expect(phoneNumberField).ToBeEmptyAsync();
            await Expect(streetField).ToBeEmptyAsync();
            await Expect(houseNumberField).ToBeEmptyAsync();
            await Expect(cityField).ToBeEmptyAsync();
            await Expect(postalCodeField).ToBeEmptyAsync();
            await Expect(countryField).Not.ToBeEmptyAsync();

            await Expect(submitButton).ToBeDisabledAsync();
            await Expect(resetButton).ToBeEnabledAsync();
        }

        [Test]
        public async Task ShouldShowValidationErrorsUponInvalidInput()
        {
            await Page.GotoAsync("/register");

            var emailField = Page.GetByTestId("email-field");
            var passwordField = Page.GetByTestId("password-field");
            var repeatPasswordField = Page.GetByTestId("repeat-password-field");
            var firstNameField = Page.GetByTestId("first-name-field");
            var lastNameField = Page.GetByTestId("last-name-field");
            var phoneNumberField = Page.GetByTestId("phone-number-field");

            var streetField = Page.GetByTestId("street-field");
            var houseNumberField = Page.GetByTestId("house-number-field");
            var cityField = Page.GetByTestId("city-field");
            var postalCodeField = Page.GetByTestId("postal-code-field");

            await emailField.FillAsync("invalid-email");
            await emailField.PressAsync("Tab");
            await Expect(emailField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await passwordField.FillAsync("short");
            await passwordField.PressAsync("Tab");
            await Expect(passwordField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await repeatPasswordField.FillAsync("different");
            await repeatPasswordField.PressAsync("Tab");
            await Expect(repeatPasswordField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await firstNameField.FillAsync("");
            await firstNameField.PressAsync("Tab");
            await Expect(firstNameField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await lastNameField.FillAsync("");
            await lastNameField.PressAsync("Tab");
            await Expect(lastNameField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await phoneNumberField.FillAsync("invalid-phone");
            await phoneNumberField.PressAsync("Tab");
            await Expect(phoneNumberField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await streetField.FillAsync("");
            await streetField.PressAsync("Tab");
            await Expect(streetField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await houseNumberField.FillAsync("");
            await houseNumberField.PressAsync("Tab");
            await Expect(houseNumberField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await cityField.FillAsync("");
            await cityField.PressAsync("Tab");
            await Expect(cityField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();

            await postalCodeField.FillAsync("");
            await postalCodeField.PressAsync("Tab");
            await Expect(postalCodeField.Locator("xpath=following-sibling::div[contains(@class, 'mud-input-control-helper-container')]//div[contains(@class, 'mud-input-helper-text') and contains(@class, 'mud-input-error')]")).ToBeVisibleAsync();
        }
    }
}