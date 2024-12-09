using MudBlazor;
using Rise.Shared.Users;
using static Rise.Shared.Users.UserProfileDto;

namespace Rise.Client.Profile.Components;

public partial class RightProfilePanel
{
    private UserProfileDto UserProfileDto = new()
    {
        Address = new()
        {
            Street = string.Empty,
            Number = string.Empty,
            City = string.Empty,
            PostalCode = string.Empty,
            Country = "Belgium"
        },
        FamilyName = string.Empty,
        FirstName = string.Empty,
        PhoneNumber = string.Empty,
    };

    private readonly UserProfileDto InitialProfileDto = new()
    {
        Address = new()
        {
            Street = "initial",
            Number = "initial",
            City = "initial",
            PostalCode = "initial",
            Country = "Belgium"
        },
        FamilyName = "initial",
        FirstName = "initial",
        PhoneNumber = "initial",
    };

    private MudForm Form = null!;

    private readonly Validator Validator = new();

    private bool EditIsEnabled = false;

    private void ToggleEdit()
    {
        EditIsEnabled = !EditIsEnabled;
    }

    private void CancelEdit()
    {
        ResetUserProfileDtoToInitialState();
        Form.ResetValidation();
        ToggleEdit();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ResetUserProfileDtoToInitialState();
    }

    private void ResetUserProfileDtoToInitialState()
    {
        UserProfileDto = new UserProfileDto
        {
            FirstName = InitialProfileDto.FirstName,
            FamilyName = InitialProfileDto.FamilyName,
            PhoneNumber = InitialProfileDto.PhoneNumber,
            Address = InitialProfileDto.Address,
        };
    }
}
