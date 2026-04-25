namespace BuscoProfe.Api.DTOs.ProfessorSkills;

public class CreateProfessorSkillDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}