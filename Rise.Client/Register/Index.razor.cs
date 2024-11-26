namespace Rise.Client.Register;

using Microsoft.AspNetCore.Components;
using Serilog;
using Rise.Shared.Users;
using FluentValidation;
using FluentValidation.Results;
using MudBlazor;

public partial class Index : ComponentBase
{
    private UserRegistrationModelDto User = new()
    {
        FirstName = string.Empty,
        LastName = string.Empty,
        Email = string.Empty,
        Password = string.Empty,
        PhoneNumber = string.Empty,
        DateOfBirth = null,
        Address = new UserRegistrationModelDto.AddressModel
        {
            Street = string.Empty,
            Number = string.Empty,
            City = string.Empty,
            PostalCode = string.Empty,
            Country = "Belgium"
        }
    };

    private bool isLoading = false;
    private bool isSuccess;

    private MudForm Form = null!;
    private UserRegistrationModelDtoValidator validator = new UserRegistrationModelDtoValidator();

    private string CheckPasswordMatch(string passwordRepeat)
    {
        if (User.Password != passwordRepeat)
        {
            return "Passwords do not match";
        }
        return string.Empty;
    }

    private async Task HandleSubmit()
    {
        isLoading = true;
        await Form.Validate();
        if (Form.IsValid)
        {
            await Task.Delay(1000);
            Log.Information(System.Text.Json.JsonSerializer.Serialize(User));
            // var response = await Http.PostAsJsonAsync("api/register", user);
            // if (response.IsSuccessStatusCode)
            // {
            isSuccess = true;
            // }
            // else
            // {
            //     errorMessage = await response.Content.ReadAsStringAsync();
            // }

            isLoading = false;
        }
        await Form.ResetAsync();
        User.Address.Country = "Belgium";
    }

    private bool isPasswordShow;
    private InputType PasswordInputType = InputType.Password;
    private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordIcon()
    {
        if (isPasswordShow)
        {
            isPasswordShow = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInputType = InputType.Password;
        }
        else
        {
            isPasswordShow = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInputType = InputType.Text;
        }
    }
}
