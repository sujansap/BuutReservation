using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;

namespace Rise.Client.Reservations.Components;

public class UserReservationsBase : ComponentBase
{
    public AsyncData<ItemsPageDto<ReservationDto>> asyncDataRef = default!;
    protected ItemsPageDto<ReservationDto>? ReservationPage { get; set; }
    protected bool IsLoading { get; private set; }

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
        await asyncDataRef.FetchData();
    }
    protected async Task LoadPreviousPage()
    {
        IsNextPage = false;
        await asyncDataRef.FetchData();
    }
}