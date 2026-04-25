using BuscoProfe.Api.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace BuscoProfe.Api.Entities;

public class JobPosting
{
    public int Id { get; set; }

    public int InstitutionUserId { get; set; }

    [ForeignKey(nameof(InstitutionUserId))]
    public User InstitutionUser { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    public string? RequirementsText { get; set; }
    public string? BenefitsText { get; set; }

    public int? SportId { get; set; }

    [ForeignKey(nameof(SportId))]
    public Sport? Sport { get; set; }

    public WorkMode WorkMode { get; set; }
    public ContractType ContractType { get; set; }
    public Availability Availability { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(100)]
    public string? Province { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(250)]
    public string? Address { get; set; }

    [MaxLength(200)]
    public string? SalaryText { get; set; }

    public JobPostingStatus Status { get; set; } = JobPostingStatus.Borrador;

    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<FavoriteJobPosting> FavoriteJobPostings { get; set; } = new List<FavoriteJobPosting>();
}