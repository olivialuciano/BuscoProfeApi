using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface ISportRepository
{
    Task<List<Sport>> GetAllAsync();
    Task<Sport?> GetByIdAsync(int id);
    Task<Sport?> GetByNameAsync(string name);
    Task<Sport> AddAsync(Sport sport);
}