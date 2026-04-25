using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);

    Task<List<User>> GetAllInstitutionsAsync();
    Task<List<User>> GetAllProfessorsAsync();
}