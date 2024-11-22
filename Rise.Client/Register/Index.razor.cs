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
        DateOfBirth = DateTime.Today,
        Address = new UserRegistrationModelDto.AddressModel
        {
            Street = string.Empty,
            Number = string.Empty,
            City = string.Empty,
            PostalCode = string.Empty,
            Country = string.Empty
        }
    };

    private bool isLoading = false;
    private bool isSuccess;
    private MudForm Form;
    private UserRegistrationModelDtoValidator validator = new UserRegistrationModelDtoValidator();

    private string Password2 { get; set; }

    private string CheckPasswordMatch(string passwordRepeat)
    {
        if (User.Password != passwordRepeat)
        {
            return "Passwords do not match";
        }
        return null;
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
    }

    bool isPasswordShow;
    InputType PasswordInputType = InputType.Password;
    string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    void TogglePasswordIcon()
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
