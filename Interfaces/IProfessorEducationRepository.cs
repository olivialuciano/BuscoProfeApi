using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IProfessorEducationRepository
{
    Task<List<ProfessorEducation>> GetByUserIdAsync(int userId);
    Task<ProfessorEducation?> GetByIdAsync(int id);
    Task<ProfessorEducation> AddAsync(ProfessorEducation entity);
    Task DeleteAsync(ProfessorEducation entity);
}