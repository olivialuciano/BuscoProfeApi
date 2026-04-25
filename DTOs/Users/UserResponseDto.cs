using BuscoProfe.Api.Enums;

namespace BuscoProfe.Api.DTOs.Users;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? LegalName { get; set; }
    public string? TradeName { get; set; }
    public ValidationStatus? ValidationStatus { get; set; }

    public string? Title { get; set; }
    public string? AboutMe { get; set; }
}