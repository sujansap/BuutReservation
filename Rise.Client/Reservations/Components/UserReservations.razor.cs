using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;

namespace Rise.Client.Reservations.Components;

public class UserReservationsBase : ComponentBase
{
    public required AsyncData<ItemsPageDto<ReservationDto>> AsyncDataRef { get; set; }
    protected ItemsPageDto<ReservationDto>? ReservationPage { get; set; }
    protected bool IsLoading { get; private set; }
    public bool HasError { get; private set; }
    protected string? ErrorMessage { get; private set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    [Inject]
    public required IReservationService ReservationService { get; set; }

    [Parameter]
    public bool IsNextPage { get; set; } = true;

    protected Task<ItemsPageDto<ReservationDto>> LoadReservations()
    {
        int? cursor = IsNextPage ? ReservationPage?.NextId : ReservationPage?.PreviousId;

        return ReservationService.GetUserReservations(
                1,
                cursor,
                IsNextPage,
                getPast: false
            );

    }

    protected async Task LoadNextPage()
    {
        IsNextPage = true;
        await AsyncDataRef.FetchData();
    }
    protected async Task LoadPreviousPage()
    {
        IsNextPage = false;
        await AsyncDataRef.FetchData();
    }

    protected void NavigateTo(int reservationId)
    {

        NavigationManager.NavigateTo($"/reservations/{reservationId}");

    }
}
