using Microsoft.AspNetCore.Components;
using Rise.Client.Services;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Microsoft.JSInterop;
using Microsoft.Extensions.Localization;
using Rise.Client.Localization.Reservations;

namespace Rise.Client.Reservations.Components;

public class UserReservationsBase : ComponentBase
{
    protected ItemsPageDto<ReservationDto>? ReservationPage { get; private set; }
    protected bool IsLoading { get; private set; }
    protected bool ShowPastReservations
    {
        get => _showPastReservations;
        set
        {
            if (_showPastReservations == value) return;
            _showPastReservations = value;
            // Reset pagination and load new reservations
            ReservationPage = null;
            LoadReservations().ConfigureAwait(false);
        }
    }
    protected bool IsFirstPage { get; private set; } = true;

    private bool _showPastReservations;

    private Stack<int?> _previousCursors = new();

    [Inject]
    public required IStringLocalizer<ReservationPageResources> Localizer { get; set; } = default!;
    [Inject]
    public required IReservationService ReservationService { get; set; }
    [Inject]
    public required IJSRuntime JS { get; set; }

    protected override async Task OnInitializedAsync() => await LoadReservations();

    private async Task LoadReservations(bool isNextPage = true)
    {
        try
        {
            IsLoading = true;
            var cursor = isNextPage ? ReservationPage?.NextId : ReservationPage?.PreviousId;
            
            if (isNextPage && cursor != null)
            {
                _previousCursors.Push(ReservationPage?.PreviousId);
            }
            else if (!isNextPage && _previousCursors.Count > 0)
            {
                cursor = _previousCursors.Pop();
            }

            IsFirstPage = cursor == null;
            
            var result = await ReservationService.GetUserReservations(
                1,
                cursor,
                cursor != null ? isNextPage : null,
                getPast: ShowPastReservations
            );

            ReservationPage = result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading reservations: {ex.Message}");
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

    protected async Task LoadNextPage()
    {
        await LoadReservations(true);
        await ScrollToTop();
    }

    protected async Task LoadPreviousPage()
    {
        await LoadReservations(false);
        await ScrollToTop();
    }

    protected async Task ScrollToTop()
    {
        await JS.InvokeVoidAsync("window.scrollTo", 0, 0);
    }

    protected bool CanGoBack => !IsFirstPage && _previousCursors.Count > 0;
}