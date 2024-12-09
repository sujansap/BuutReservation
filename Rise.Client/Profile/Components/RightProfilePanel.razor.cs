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

    private UserProfileDto InitialProfileDto = new()
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
        PhoneNumber = "911",
    };

    private MudForm Form = null!;

    private readonly Validator Validator = new();

    private bool EditIsEnabled = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetUserProfileDtoToInitialState();
    }

    private void SaveChanges()
    {
        Form.Validate();
        if (Form.IsValid)
        {
            SetInitialStateToCurrentUserProfileDto();
            Form.ResetValidation();
            ToggleEdit();
        }
    }

    private void CancelEdit()
    {
        SetUserProfileDtoToInitialState();
        Form.ResetValidation();
        ToggleEdit();
    }

    private void ToggleEdit()
    {
        EditIsEnabled = !EditIsEnabled;
    }


    private void SetUserProfileDtoToInitialState()
    {
        UserProfileDto = new UserProfileDto
        {
            FirstName = InitialProfileDto.FirstName,
            FamilyName = InitialProfileDto.FamilyName,
            PhoneNumber = InitialProfileDto.PhoneNumber,
            Address = InitialProfileDto.Address,
        };
    }

    private void SetInitialStateToCurrentUserProfileDto()
    {
        InitialProfileDto = new UserProfileDto
        {
            FirstName = UserProfileDto.FirstName,
            FamilyName = UserProfileDto.FamilyName,
            PhoneNumber = UserProfileDto.PhoneNumber,
            Address = UserProfileDto.Address,
        };
    }
}
