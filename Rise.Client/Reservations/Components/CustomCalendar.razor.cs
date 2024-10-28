using System;
using System.Globalization;
using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;

namespace Rise.Client.Reservations.Components
{
  public partial class CustomCalendar
  {
    private DateTime? PickerDate
    {
      get => CurrentDay;
      set => CurrentDay = value ?? DateTime.Today;
    }

    private CalendarDateRange? _currentDateRange;

    private CalendarDatePicker? _datePicker;
    [Parameter]
    public List<DateTime> GreyedOutDates { get; set; } = [];

    [Parameter]
    public bool AllowPast { get; set; } = false;

    protected override void OnInitialized()
    {
      base.OnInitialized();
    }

    protected override async Task OnPreviousClicked()
    {
      if (CanGoToPreviousRange())
      {
        await base.OnPreviousClicked();
      }
    }

    /// <summary>
    /// If the view can go back to the previous range 
    /// </summary>
    /// <returns>If the view can go back to the previous range</returns>
    private bool CanGoToPreviousRange()
    {
      if (AllowPast)
      {
        return true;
      }

      DateTime newDate = View switch
      {
        CalendarView.Day => CurrentDay.AddDays(-1),
        CalendarView.Week => CurrentDay.AddDays(-7),
        CalendarView.WorkWeek => CurrentDay.AddDays(-7),
        CalendarView.Month => CurrentDay.AddMonths(-1),
        _ => CurrentDay
      };

      return IsBeforeCurrentMonth(newDate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    private static bool IsBeforeCurrentMonth(DateTime date)
    {
      return date.CompareTo(DateTime.Today.StartOfMonth(CultureInfo.CurrentCulture)) >= 0;
    }

    /// <summary>
    /// Method from original component: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L599
    /// </summary>
    /// <param name="dateTime">The selected date picker</param>
    /// <returns></returns>
    private async Task DatePickerDateChanged(DateTime? dateTime)
    {
      var dateChanged = dateTime.HasValue && dateTime != CurrentDay;

      PickerDate = dateTime;

      if (dateChanged) await CurrentDayChanged.InvokeAsync(CurrentDay);

      await ChangeDateRange(new CalendarDateRange(dateTime ?? DateTime.Today, View, GetFirstDayOfWeekByCalendarView(View)));
    }

    /// <summary>
    /// Method from original component since it's private: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L620
    /// </summary>
    /// <param name="dateRange">The selected date range</param>
    /// <returns></returns>
    private async Task ChangeDateRange(CalendarDateRange dateRange)
    {
      if (dateRange != _currentDateRange)
      {
        _currentDateRange = dateRange;
        await DateRangeChanged.InvokeAsync(dateRange);
      }
    }

    /// <summary>
    /// Method from original component since it's private: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L610
    /// </summary>
    private void OnDatePickerOpened()
    {
      _datePicker?.GoToDate(CurrentDay);
    }

    /// <summary>
    /// Method from original component since it's private: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L629
    /// </summary>
    /// <returns>Allowed views</returns>
    private List<CalendarView> AllowedViews()
    {
      var list = new List<CalendarView>();
      if (ShowDay) list.Add(CalendarView.Day);
      if (ShowWeek) list.Add(CalendarView.Week);
      if (ShowWorkWeek) list.Add(CalendarView.WorkWeek);
      if (ShowMonth) list.Add(CalendarView.Month);
      return list;
    }

  }
}


