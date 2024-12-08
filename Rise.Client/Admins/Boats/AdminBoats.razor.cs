using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Client.Common;
using Rise.Shared;

namespace Rise.Client.Admins.Boats
{
    public partial class AdminBoats
    {
        // public required AsyncData<List<BoatDto>> AsyncDataRef { get; set; }

        private List<BoatDto>? boats;

        [Inject]
        public required IBoatService boatService { get; set; }

        [Inject]
        public required ISnackbar snackbar { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadBoats();
        }

        private async Task LoadBoats()
        {
            try
            {
                boats = (await boatService.GetAllBoatsAsync()).ToList();
            }
            catch (Exception ex)
            {
                snackbar.Add($"Error loading boats: {ex.Message}", Severity.Error);
            }
        }

        private void EditBoat(int boatId)
        {
            // TODO: Implement EditBoat logic
        }

        private async Task DeleteBoat(int boatId)
        {
            // TODO: Implement DeleteBoat logic
        }

        private void AddBoat()
        {
            // TODO: Implement AddBoat logic
        }
    }
}
