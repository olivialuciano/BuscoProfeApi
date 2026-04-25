using BuscoProfe.Api.Enums;

namespace BuscoProfe.Api.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}