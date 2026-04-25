using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IProfessorExperienceRepository
{
    Task<List<ProfessorExperience>> GetByUserIdAsync(int userId);
    Task<ProfessorExperience?> GetByIdAsync(int id);
    Task<ProfessorExperience> AddAsync(ProfessorExperience entity);
    Task DeleteAsync(ProfessorExperience entity);
}