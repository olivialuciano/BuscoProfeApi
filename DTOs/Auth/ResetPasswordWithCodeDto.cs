namespace BuscoProfe.Api.DTOs.Auth;

public class ResetPasswordWithCodeDto
{
    public string Email { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string NewPassword { get; set; } = string.Empty;

    public string ConfirmNewPassword { get; set; } = string.Empty;
}