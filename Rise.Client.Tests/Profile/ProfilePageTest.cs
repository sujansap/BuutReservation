using System;
using Microsoft.Playwright;
using Rise.Shared.Address;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Profile;

public class ProfilePageTest : CustomAuthenticatedPageTest
{
    private const string ProfilePageUrl = "/profile";

    private readonly UserProfileDto profileDto = new()
    {
        Email = "guest@guest.com",
        FirstName = "Guest",
        FamilyName = "GuestFamilyName",
        PhoneNumber = "0123456789",
        Address = new AddressDto()
        {
            Street = "StreetName",
            City = "Belgium",
            Country = "CountryName",
            Number = "StreetNumber",
            PostalCode = "CityPostalCode",
        },
        DateOfBirth = new DateTime(2000, 1, 1)
    };

    [SetUp]
    public async Task SetUp()
    {
        await LoginAsync(UserRole.Guest);
    }

    private async Task MockProfileApi(int delayMs = 0)
    {
        await Page.RouteAsync($"*/**/api/User/profile**", async route =>
        {
            if (delayMs > 0)
                await Task.Delay(delayMs);

            var response = profileDto;

            await route.FulfillAsync(new()
            {
                ContentType = "application/json",
                Body = System.Text.Json.JsonSerializer.Serialize(response)
            });
        });
    }

    private async Task MockProfileApiError()
    {
        await Page.RouteAsync($"*/**/api/User/profile**", async route =>
        {
            await route.FulfillAsync(new() { Status = 404, Body = "Not found" });
        });
    }

    [Test]
    public async Task ShowsLoadingStateWhileFetchingProfile()
    {
        await MockProfileApi(5000);

        await NavigateToUrl(ProfilePageUrl);

        await Expect(Page.GetByTestId("profile-loading-progress")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShowsErrorStateWhenApiReturns404()
    {
        await MockProfileApiError();
        await NavigateToUrl(ProfilePageUrl);

        await Page.WaitForSelectorAsync("[data-testid='profile-loading-progress']", new() { State = WaitForSelectorState.Hidden });
        await Expect(Page.GetByTestId("profile-fetch-error")).ToBeVisibleAsync();
        await Page.WaitForSelectorAsync("[data-testid='profile-fetch-error']", new() { State = WaitForSelectorState.Visible });

        ILocator errorMessage = Page.GetByTestId("profile-fetch-error");
        await Expect(errorMessage).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShowsUserProfile()
    {
        await MockProfileApi(5000);
        await NavigateToUrl(ProfilePageUrl);

        await Page.WaitForSelectorAsync("[data-testid='profile-loading-progress']", new() { State = WaitForSelectorState.Visible });

        await Expect(Page.GetByTestId("profile-loading-progress")).ToBeVisibleAsync();

        await Page.WaitForSelectorAsync("[data-testid='profile-loading-progress']", new() { State = WaitForSelectorState.Hidden });

        await Expect(Page.GetByTestId("profile-full-name")).ToHaveTextAsync($"{profileDto.FirstName} {profileDto.FamilyName}");

        var dateOfBirth = Page.GetByTestId("profile-date-of-birth");
        await Expect(dateOfBirth).ToBeVisibleAsync();
        await Expect(dateOfBirth).ToHaveTextAsync($"{profileDto.DateOfBirth:dd/MM/yyyy}");

        await Expect(Page.GetByTestId("profile-roles")).ToHaveTextAsync($"{nameof(UserRole.Guest)}");
        await Expect(Page.GetByTestId("profile-first-name")).ToHaveTextAsync($"{profileDto.FirstName}");
        await Expect(Page.GetByTestId("profile-last-name")).ToHaveTextAsync($"{profileDto.FamilyName}");
        await Expect(Page.GetByTestId("profile-phone-number")).ToHaveTextAsync($"{profileDto.PhoneNumber}");
        await Expect(Page.GetByTestId("profile-street")).ToHaveTextAsync($"{profileDto.Address.Street}");
        await Expect(Page.GetByTestId("profile-number")).ToHaveTextAsync($"{profileDto.Address.Number}");
        await Expect(Page.GetByTestId("profile-city")).ToHaveTextAsync($"{profileDto.Address.City}");
        await Expect(Page.GetByTestId("profile-postal-code")).ToHaveTextAsync($"{profileDto.Address.PostalCode}");
        await Expect(Page.GetByTestId("profile-country")).ToHaveTextAsync($"{profileDto.Address.Country}");
    }

}
