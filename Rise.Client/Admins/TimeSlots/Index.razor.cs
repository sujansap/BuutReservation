
using MudBlazor;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata.Ecma335;

namespace Rise.Client.Admins.TimeSlots
{
    public partial class Index : ComponentBase
    {
        private TimeSpan? StartTime { get; set; }
        private TimeSpan? EndTime { get; set; }
        private HashSet<TimeSlot> TimeSlots { get; set; } = new();

        private bool CanAddTimeSlot =>
            StartTime.HasValue &&
            EndTime.HasValue &&
            EndTime.Value > StartTime.Value;

        private CruisePeriod CruisePeriodExm { get; set; } = new()
        {
            Name = "Summer Season 2024",
            StartDate = new DateTime(2024, 6, 1),
            EndDate = new DateTime(2024, 8, 31)
        };

        private void AddTimeSlot()
        {
            if (CanAddTimeSlot)
            {
                var timeSlot = new TimeSlot(StartTime.Value, EndTime.Value);
                TimeSlots.Add(timeSlot);

                // Reset inputs
                StartTime = null;
                EndTime = null;

                StateHasChanged();
            }
        }

        private void RemoveTimeSlot(TimeSlot timeSlot)
        {
            TimeSlots.Remove(timeSlot);
            StateHasChanged();
        }

        private async Task SaveTimeSlots()
        {
            // TODO: Add your save logic here
        }
    }






}

public class CruisePeriod
{
    public required string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}


public record TimeSlot(TimeSpan Start, TimeSpan End)
{
    public override string ToString() => $"{Start:hh\\:mm} - {End:hh\\:mm}";
}