namespace BuscoProfe.Api.DTOs.Favorites;

public class CreateFavoriteInstitutionDto
{
    public int ProfessorUserId { get; set; }
    public int InstitutionUserId { get; set; }
}