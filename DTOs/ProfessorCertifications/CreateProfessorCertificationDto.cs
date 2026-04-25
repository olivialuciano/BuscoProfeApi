namespace BuscoProfe.Api.DTOs.ProfessorCertifications;

public class CreateProfessorCertificationDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public DateOnly? IssueDate { get; set; }
    public string? CredentialUrl { get; set; }
}