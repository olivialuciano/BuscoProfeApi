using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IProfessorSkillRepository
{
    Task<List<ProfessorSkill>> GetByUserIdAsync(int userId);
    Task<ProfessorSkill?> GetByIdAsync(int id);
    Task<ProfessorSkill> AddAsync(ProfessorSkill entity);
    Task DeleteAsync(ProfessorSkill entity);
}