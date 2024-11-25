using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Rise.Client.Tests
{
    public enum UserRole
    {
        Admin,
        Guest,
        Test
    }

    public class LoginFixture : CustomAuthenticatedPageTest
    {
        private readonly string _authStatePath = "./Playwright/.auth/state.json";

        public async Task LoginAsync(UserRole role)
        {
            var credentials = role switch
            {
                UserRole.Admin => Builder.Configuration.GetSection("Admin").Get<Credentials>(),
                UserRole.Guest => Builder.Configuration.GetSection("Guest").Get<Credentials>(),
                UserRole.Test => Builder.Configuration.GetSection("Test").Get<Credentials>(),
                _ => throw new ArgumentOutOfRangeException()
            };

            await Page.GotoAsync("authentication/login");

            if (credentials == null)
            {
                throw new InvalidOperationException("Credentials cannot be null");
            }

            await Page.FillAsync("input[name='username']", credentials.Email);
            await Page.FillAsync("input[name='password']", credentials.Password);
            await Page.ClickAsync("button[type='submit']");
            await Page.WaitForURLAsync("authentication/callback");

            var sessionStorage = await Page.EvaluateAsync<string>("() => JSON.stringify(sessionStorage)");
            Environment.SetEnvironmentVariable("SESSION_STORAGE", sessionStorage);

            var loadedSessionStorage = Environment.GetEnvironmentVariable("SESSION_STORAGE");
            await Context.AddInitScriptAsync(@"(storage => {
                if (window.location.hostname === 'example.com') {
                    const entries = JSON.parse(storage);
                    for (const [key, value] of Object.entries(entries)) {
                        window.sessionStorage.setItem(key, value);
                    }
                }
            })('" + loadedSessionStorage + "')");

            var storageState = await Context.StorageStateAsync();
            await File.WriteAllTextAsync(_authStatePath, storageState);
        }

        private class Credentials
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
