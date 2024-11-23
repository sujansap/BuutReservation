using Microsoft.EntityFrameworkCore;
using Rise.Domain.TimeSlots;

namespace Rise.Persistence.Seeders;

internal class CruisePeriodSeeder(ApplicationDbContext dbContext) : GeneralSeeder<CruisePeriod>(dbContext)
{
    /// <summary>
    /// Cruise period that started one month ago and ends until two days before today.
    /// </summary>
    internal static readonly CruisePeriod PastMonthLong = new() { Start = DateTime.Now.AddMonths(-1), End = DateTime.Now.AddDays(-2) };

    /// <summary>
    /// Cruise period that starts now for a full week.
    /// </summary>
    internal static readonly CruisePeriod WeekLong = new() { Start = DateTime.Now, End = DateTime.Now.AddDays(7) };

    /// <summary>
    /// Cruise period that starts nine days from now for two weeks and 2 days. 
    /// </summary>
    internal static readonly CruisePeriod TwoWeekAndTwoDaysLong = new() { Start = DateTime.Now.AddDays(9), End = DateTime.Now.AddDays(25) };

    /// <summary>
    /// Cruise period that starts next month from now for a month. 
    /// </summary>
    internal static readonly CruisePeriod MonthLong = new() { Start = DateTime.Now.AddMonths(1), End = DateTime.Now.AddMonths(2) };

    protected override DbSet<CruisePeriod> DbSet => _dbContext.CruisePeriods;
    protected override IEnumerable<CruisePeriod> Items => [
        PastMonthLong,
        WeekLong,
        TwoWeekAndTwoDaysLong,
        MonthLong,
    ];
}