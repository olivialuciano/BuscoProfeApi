namespace BuscoProfe.Api.DTOs.ProfessorExperiences;

public class CreateProfessorExperienceDto
{
    public int UserId { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public int? SportId { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }
}