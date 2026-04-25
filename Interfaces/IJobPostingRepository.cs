using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IJobPostingRepository
{
    Task<List<JobPosting>> GetAllAsync();
    Task<List<JobPosting>> GetByInstitutionUserIdAsync(int institutionUserId);
    Task<JobPosting?> GetByIdAsync(int id);
    Task<JobPosting> AddAsync(JobPosting entity);
    Task UpdateAsync(JobPosting entity);
    Task<bool> InstitutionHasAnyAsync(int institutionUserId);
}