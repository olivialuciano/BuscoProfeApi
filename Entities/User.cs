using BuscoProfe.Api.DTOs;
using BuscoProfe.Api.Enums;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace BuscoProfe.Api.Entities;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    [MaxLength(100)]
    public string? FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; }

    [MaxLength(50)]
    public string? WhatsApp1 { get; set; }

    [MaxLength(50)]
    public string? WhatsApp2 { get; set; }

    [MaxLength(50)]
    public string? WhatsApp3 { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    [MaxLength(100)]
    public string? Province { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(250)]
    public string? Address { get; set; }

    [MaxLength(500)]
    public string? ProfileImageUrl { get; set; }

    [MaxLength(500)]
    public string? CoverImageUrl { get; set; }

    // Institution fields
    [MaxLength(200)]
    public string? LegalName { get; set; }

    [MaxLength(200)]
    public string? TradeName { get; set; }

    public InstitutionType? InstitutionType { get; set; }

    [MaxLength(300)]
    public string? ShortDescription { get; set; }

    public string? Description { get; set; }

    [MaxLength(500)]
    public string? Website { get; set; }

    [MaxLength(500)]
    public string? InstagramUrl { get; set; }

    [MaxLength(500)]
    public string? FacebookUrl { get; set; }

    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }

    public string? BenefitsText { get; set; }
    public string? ValuesText { get; set; }
    public string? HiringInfoText { get; set; }

    public ValidationStatus? ValidationStatus { get; set; }
    public string? RejectionReason { get; set; }

    // Professor fields
    [MaxLength(200)]
    public string? Title { get; set; }

    public string? AboutMe { get; set; }

    [MaxLength(300)]
    public string? Languages { get; set; }

    [MaxLength(300)]
    public string? PreferredZone { get; set; }

    public Availability? Availability { get; set; }
    public WorkMode? WorkModePreference { get; set; }
    public ContractType? ContractPreference { get; set; }

    [MaxLength(200)]
    public string? SalaryExpectationText { get; set; }

    [MaxLength(500)]
    public string? CvUrl { get; set; }

    public bool? IsPublic { get; set; }
    public bool EmailConfirmed { get; set; } = false;

    public string? EmailVerificationCode { get; set; }

    public DateTime? EmailVerificationCodeExpiresAt { get; set; }

    public DateTime? EmailVerificationCodeLastSentAt { get; set; }

    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public ICollection<ProfessorExperience> Experiences { get; set; } = new List<ProfessorExperience>();
    public ICollection<ProfessorEducation> Educations { get; set; } = new List<ProfessorEducation>();
    public ICollection<ProfessorCertification> Certifications { get; set; } = new List<ProfessorCertification>();
    public ICollection<ProfessorSkill> Skills { get; set; } = new List<ProfessorSkill>();
    public ICollection<FavoriteJobPosting> FavoriteJobPostings { get; set; } = new List<FavoriteJobPosting>();
    public ICollection<FavoriteInstitution> FavoriteInstitutions { get; set; } = new List<FavoriteInstitution>();
}