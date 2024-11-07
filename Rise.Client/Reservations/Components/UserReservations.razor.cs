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

    protected bool ShowPastReservations
    {
        get => _showPastReservations;
        set
        {
            if (_showPastReservations == value) return;
            _showPastReservations = value;
            // Reset pagination and load new reservations
            ReservationPage = null;
            IsFirstPage = true;
            _previousCursors.Clear();  // Clear the cursor history
            AsyncDataRef.FetchData().ConfigureAwait(false);
        }
    }
    protected bool IsFirstPage { get; private set; } = true;

    private bool _showPastReservations;

    [Inject]
    public required IStringLocalizer<ReservationPageResources> Localizer { get; set; } = default!;
    [Inject]
    public required IReservationService ReservationService { get; set; }
    [Inject]
    public required IJSRuntime JS { get; set; }

    [Parameter]
    public bool IsNextPage { get; set; } = true;

    private Stack<int> _previousCursors = new();

    protected Task<ItemsPageDto<ReservationDto>> LoadReservations()
    {
        int? cursor = IsNextPage ? ReservationPage?.NextId : ReservationPage?.PreviousId;

        return ReservationService.GetUserReservations(
                1,
                cursor,
                IsNextPage,
                getPast: ShowPastReservations
            );
    }

    protected async Task LoadNextPage()
    {
        if (ReservationPage?.NextId != null)
        {
            _previousCursors.Push(ReservationPage.Data.First().Id);
            IsFirstPage = false;
            IsNextPage = true;
            await AsyncDataRef.FetchData();
            await ScrollToTop();
        }
    }

    protected async Task LoadPreviousPage()
    {
        if (_previousCursors.Count > 0)
        {
            IsNextPage = false;
            await AsyncDataRef.FetchData();
            _previousCursors.Pop();
            IsFirstPage = _previousCursors.Count == 0;
            await ScrollToTop();
        }
    }

    protected async Task ScrollToTop()
    {
        await JS.InvokeVoidAsync("window.scrollTo", 0, 0);
    }

    protected bool CanGoBack => !IsFirstPage && _previousCursors.Count > 0;

    public async Task FetchData()
    {
        try
        {
            IsLoading = true;
            HasError = false;
            ErrorMessage = null;
            ReservationPage = await LoadReservations();
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = Localizer["ErrorLoadingReservations"];
        }
        finally
        {
            IsLoading = false;
        }
    }
}