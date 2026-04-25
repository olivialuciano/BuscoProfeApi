using BuscoProfe.Api.Enums;

namespace BuscoProfe.Api.DTOs.Users;

public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public string? WhatsApp1 { get; set; }
    public string? WhatsApp2 { get; set; }
    public string? WhatsApp3 { get; set; }

    public string? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? Address { get; set; }

    public string? ProfileImageUrl { get; set; }
    public string? CoverImageUrl { get; set; }

    public string? LegalName { get; set; }
    public string? TradeName { get; set; }
    public InstitutionType? InstitutionType { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? InstagramUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? BenefitsText { get; set; }
    public string? ValuesText { get; set; }
    public string? HiringInfoText { get; set; }

    public string? Title { get; set; }
    public string? AboutMe { get; set; }
    public string? Languages { get; set; }
    public string? PreferredZone { get; set; }
    public Availability? Availability { get; set; }
    public WorkMode? WorkModePreference { get; set; }
    public ContractType? ContractPreference { get; set; }
    public string? SalaryExpectationText { get; set; }
    public string? CvUrl { get; set; }
    public bool? IsPublic { get; set; }
}