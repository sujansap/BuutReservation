namespace Rise.Domain.Common;

public record TimeInfo(
    DateTime Now,
    DateOnly Today,
    TimeOnly CurrentTime,
    DateOnly ThreeDaysFromNow);