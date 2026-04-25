using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoProfe.Api.Entities;

public class ProfessorCertification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Issuer { get; set; }

    public DateOnly? IssueDate { get; set; }

    [MaxLength(500)]
    public string? CredentialUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}