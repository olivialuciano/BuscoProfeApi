using BuscoProfe.Api.Enums;

namespace BuscoProfe.Api.DTOs.JobPostings;

public class CreateJobPostingDto
{
    public int InstitutionUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? RequirementsText { get; set; }
    public string? BenefitsText { get; set; }
    public int? SportId { get; set; }
    public WorkMode WorkMode { get; set; }
    public ContractType ContractType { get; set; }
    public Availability Availability { get; set; }
    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }
    public string? SalaryText { get; set; }
}