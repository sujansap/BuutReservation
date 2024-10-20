using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components.TimeSlotList
{
    public partial class TimeSlotComponent
    {
        [Parameter]
        public TimeSlotDto? Timeslot { get; set; }

        private string StackStyle { get; set; } = GetStyle(AvailabilityEnum.Unavailable);
        private Color TextColor { get; set; } = GetColor(AvailabilityEnum.Unavailable);

        protected override void OnParametersSet()
        {
            StackStyle = GetStyle(AvailabilityEnum.Available);
            TextColor = GetColor(AvailabilityEnum.Available);
        }

        public static Color GetColor(AvailabilityEnum availability)
        {
            return availability switch
            {
                AvailabilityEnum.Available => Color.Default,
                AvailabilityEnum.Unavailable => Color.Error,
                AvailabilityEnum.Booked => Color.Primary,
                _ => Color.Default
            };
        }

        public static string GetStyle(AvailabilityEnum availability)
        {
            return new StyleBuilder()
            .AddStyle("background-color", availability switch
            {
                AvailabilityEnum.Available => "rgba(var(--mud-palette-dark-rgb), 0.1)",
                AvailabilityEnum.Unavailable => "rgba(var(--mud-palette-error-rgb), 0.1)",
                AvailabilityEnum.Booked => "rgba(var(--mud-palette-primary-rgb), 0.1)",
                _ => "rgba(var(--mud-palette-grey-rgb), 0.1)"
            })
            .Build();
        }

        public enum AvailabilityEnum
        {
            Available = 0,
            Unavailable = 1,
            Booked = 2

        }
    }
}