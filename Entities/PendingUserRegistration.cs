using BuscoProfe.Api.Enums;

namespace BuscoProfe.Api.Entities;

public class PendingUserRegistration
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? LegalName { get; set; }

    public string? TradeName { get; set; }

    public InstitutionType? InstitutionType { get; set; }

    public string? City { get; set; }

    public string? Province { get; set; }

    public string? Country { get; set; }

    public string EmailVerificationCode { get; set; } = string.Empty;

    public DateTime EmailVerificationCodeExpiresAt { get; set; }

    public DateTime EmailVerificationCodeLastSentAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}