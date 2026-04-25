using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IApplicationRepository
{
    Task<List<Application>> GetAllAsync();
    Task<List<Application>> GetByProfessorUserIdAsync(int professorUserId);
    Task<Application?> GetByIdAsync(int id);
    Task<Application?> GetByIdAsync2(int id);
    Task<Application?> GetByJobPostingAndProfessorAsync(int jobPostingId, int professorUserId);
    Task<Application> AddAsync(Application entity);
    Task UpdateAsync(Application entity);
    Task AcceptAsync(Application application);
    Task RejectAsync(Application application);
}