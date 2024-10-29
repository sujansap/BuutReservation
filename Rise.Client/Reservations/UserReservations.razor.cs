using Microsoft.AspNetCore.Components;
using Rise.Client.Services;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;

namespace Rise.Client.Reservations.Components;

public class UserReservationsBase : ComponentBase
{
    protected ItemsPageDto<ReservationDto>? ReservationPage { get; private set; }
    protected bool IsLoading { get; private set; }

    [Inject]
    public required IReservationService ReservationService { get; set; }

    protected override async Task OnInitializedAsync() => await LoadReservations();

    private async Task LoadReservations(bool isNextPage = true)
    {
        try
        {
            IsLoading = true;
            var cursor = isNextPage ? ReservationPage?.NextId : ReservationPage?.PreviousId;

            var result = await ReservationService.GetUserReservations(
                1,
                cursor,
                isNextPage,
                getPast: false
            );

            ReservationPage = result;
        }
        catch (Exception ex)
        {
            // proper error handling/logging needed here
            ReservationPage = new()
            {
                Data = new List<ReservationDto>()
            };
        }
        finally
        {
            IsLoading = false;
            StateHasChanged();
        }
    }

    protected Task LoadNextPage() => LoadReservations(true);
    protected Task LoadPreviousPage() => LoadReservations(false);
} 