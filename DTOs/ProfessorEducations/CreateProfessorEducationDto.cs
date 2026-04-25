namespace BuscoProfe.Api.DTOs.ProfessorEducations;

public class CreateProfessorEducationDto
{
    public int UserId { get; set; }
    public string InstitutionName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Status { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Description { get; set; }
}