namespace BuscoProfe.Api.DTOs.Favorites;

public class CreateFavoriteJobPostingDto
{
    public int ProfessorUserId { get; set; }
    public int JobPostingId { get; set; }
}