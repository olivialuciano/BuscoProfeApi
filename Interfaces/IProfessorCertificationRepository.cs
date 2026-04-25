using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IProfessorCertificationRepository
{
    Task<List<ProfessorCertification>> GetByUserIdAsync(int userId);
    Task<ProfessorCertification?> GetByIdAsync(int id);
    Task<ProfessorCertification> AddAsync(ProfessorCertification entity);
    Task DeleteAsync(ProfessorCertification entity);
}