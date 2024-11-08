using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using Rise.Client.Localization.Reservations;

namespace Rise.Client.Reservations.Components;

public class UserReservationsBase : ComponentBase
{
    public required AsyncData<ItemsPageDto<ReservationDto>> AsyncDataRef { get; set; }
    protected ItemsPageDto<ReservationDto>? ReservationPage { get; set; }
    protected bool IsLoading { get; private set; }
    public bool HasError { get; private set; }
    protected string? ErrorMessage { get; private set; }

    [Inject]
    public required IReservationService ReservationService { get; set; }

    [Inject]
    public required IStringLocalizer<ReservationPageResources> Localizer { get; set; } = default!;

    [Inject]
    public required IJSRuntime JS { get; set; } 

    [Parameter]
    public bool IsNextPage { get; set; } = true;

    private bool _showPastReservations;
    private Stack<int?> _previousCursors = new();
    private bool IsFirstPage;

    protected bool ShowPastReservations
    {
        get => _showPastReservations;
        set
        {
            if (_showPastReservations == value) return;
            _showPastReservations = value;
            AsyncDataRef?.FetchData();
        }
    }

    protected Task<ItemsPageDto<ReservationDto>> LoadReservations()
    {
        int? cursor = IsNextPage ? ReservationPage?.NextId : ReservationPage?.PreviousId;
        
        if (IsNextPage && cursor != null)
        {
            _previousCursors.Push(ReservationPage?.PreviousId);
        }
        else if (!IsNextPage && _previousCursors.Count > 0)
        {
            cursor = _previousCursors.Pop();
        }

        IsFirstPage = cursor == null;

        return ReservationService.GetUserReservations(
            1,
            cursor,
            cursor != null ? IsNextPage : null,
            ShowPastReservations,
            pageSize: 5
        );
    }

    protected async Task LoadNextPage()
    {
        IsNextPage = true;
        await AsyncDataRef.FetchData();
        await ScrollToTop();
    }

    protected async Task LoadPreviousPage()
    {    
        if (ReservationPage?.PreviousId is null || ReservationPage?.IsFirstPage == true)
            return;
        
        IsNextPage = false;
        await AsyncDataRef.FetchData();
        await ScrollToTop(); 
    }

    protected async Task ScrollToTop()
    {
        await JS.InvokeVoidAsync("window.scrollTo", 0, 0);
    }
}