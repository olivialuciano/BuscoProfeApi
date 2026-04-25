using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoProfe.Api.Entities;

public class ProfessorExperience
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Required, MaxLength(200)]
    public string InstitutionName { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Position { get; set; } = string.Empty;

    public int? SportId { get; set; }

    [ForeignKey(nameof(SportId))]
    public Sport? Sport { get; set; }

    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}