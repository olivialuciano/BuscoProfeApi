namespace BuscoProfe.Api.DTOs.Applications;

public class CreateApplicationDto
{
    public int JobPostingId { get; set; }
    public int ProfessorUserId { get; set; }
    public string? Message { get; set; }
    public string? CvUrl { get; set; }
}