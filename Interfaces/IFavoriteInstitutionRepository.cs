using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IFavoriteInstitutionRepository
{
    Task<List<FavoriteInstitution>> GetByProfessorUserIdAsync(int professorUserId);
    Task<FavoriteInstitution?> GetByIdAsync(int id);
    Task<FavoriteInstitution?> GetByProfessorAndInstitutionAsync(int professorUserId, int institutionUserId);
    Task<FavoriteInstitution> AddAsync(FavoriteInstitution entity);
    Task DeleteAsync(FavoriteInstitution entity);
    Task<List<FavoriteInstitution>> GetByInstitutionUserIdAsync(int institutionUserId);
}