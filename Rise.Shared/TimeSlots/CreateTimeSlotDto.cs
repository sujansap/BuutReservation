using System;

namespace Rise.Shared.TimeSlots;
public record CreateTimeSlotDto(
    TimeOnly Start,
    TimeOnly End,
    DateOnly Date,
    int CruisePeriodId
);
