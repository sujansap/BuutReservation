using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;

namespace Rise.Client.Components
{
    public partial class CultureSelector
    {
        [Parameter]
        public Color MenuColor { get; set; } = Color.Secondary;

        private CultureInfo[] supportedCultures = new[]
        {
        new CultureInfo("nl-BE"),
        new CultureInfo("en-US"),
        };

        private CultureInfo? selectedCulture;

        protected override void OnInitialized()
        {
            selectedCulture = CultureInfo.CurrentCulture;
        }

        private async Task OnCultureSelectedAsync(CultureInfo culture)
        {
            selectedCulture = culture;
            await ApplySelectedCultureAsync();
        }

        private async Task ApplySelectedCultureAsync()
        {
            if (CultureInfo.CurrentCulture != selectedCulture)
            {
                await JS.InvokeVoidAsync("blazorCulture.set", selectedCulture!.Name);
                Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
            }
        }
    }
}