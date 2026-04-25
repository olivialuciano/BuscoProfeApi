using BuscoProfe.Api.Enums;

namespace BuscoProfe.Api.DTOs.Auth;

public class StartRegistrationDto
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? LegalName { get; set; }

    public string? TradeName { get; set; }

    public InstitutionType? InstitutionType { get; set; }

    public string? City { get; set; }

    public string? Province { get; set; }

    public string? Country { get; set; }
}