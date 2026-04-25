using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IFavoriteJobPostingRepository
{
    Task<List<FavoriteJobPosting>> GetByProfessorUserIdAsync(int professorUserId);
    Task<FavoriteJobPosting?> GetByIdAsync(int id);
    Task<FavoriteJobPosting?> GetByProfessorAndJobPostingAsync(int professorUserId, int jobPostingId);
    Task<FavoriteJobPosting> AddAsync(FavoriteJobPosting entity);
    Task DeleteAsync(FavoriteJobPosting entity);

    Task DeleteByJobPostingIdAsync(int jobPostingId);
}