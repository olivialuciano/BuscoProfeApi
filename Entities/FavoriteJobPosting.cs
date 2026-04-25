using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoProfe.Api.Entities;

public class FavoriteJobPosting
{
    public int Id { get; set; }

    public int ProfessorUserId { get; set; }

    [ForeignKey(nameof(ProfessorUserId))]
    public User ProfessorUser { get; set; } = null!;

    public int JobPostingId { get; set; }

    [ForeignKey(nameof(JobPostingId))]
    public JobPosting JobPosting { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}