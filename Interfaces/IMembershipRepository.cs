using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IMembershipRepository
{
    Task<Membership?> GetByIdAsync(int id);
    Task<Membership?> GetActiveByInstitutionUserIdAsync(int institutionUserId);
    Task<List<Membership>> GetByInstitutionUserIdAsync(int institutionUserId);
    Task<Membership> AddAsync(Membership entity);
    Task UpdateAsync(Membership entity);
}