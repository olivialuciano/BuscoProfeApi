using BuscoProfe.Api.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuscoProfe.Api.Entities;

public class Application
{
    public int Id { get; set; }

    public int JobPostingId { get; set; }

    [ForeignKey(nameof(JobPostingId))]
    public JobPosting JobPosting { get; set; } = null!;

    public int ProfessorUserId { get; set; }

    [ForeignKey(nameof(ProfessorUserId))]
    public User ProfessorUser { get; set; } = null!;

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Aplicado;

    [MaxLength(1500)]
    public string? Message { get; set; }

    [MaxLength(500)]
    public string? CvUrl { get; set; }

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}