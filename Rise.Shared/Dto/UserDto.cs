namespace Rise.Shared.Dto;

public record UserDto : BaseDto
{
    public string FamilyName { get; } = default!;
}