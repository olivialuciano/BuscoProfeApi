using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoProfe.Api.Entities;

public class FavoriteInstitution
{
    public int Id { get; set; }

    public int ProfessorUserId { get; set; }

    [ForeignKey(nameof(ProfessorUserId))]
    public User ProfessorUser { get; set; } = null!;

    public int InstitutionUserId { get; set; }

    [ForeignKey(nameof(InstitutionUserId))]
    public User InstitutionUser { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}