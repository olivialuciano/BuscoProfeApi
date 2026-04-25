using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class SportRepository : ISportRepository
{
    private readonly AppDbContext _context;

    public SportRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Sport>> GetAllAsync()
        => await _context.Sports.OrderBy(x => x.Name).ToListAsync();

    public async Task<Sport?> GetByIdAsync(int id)
        => await _context.Sports.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Sport?> GetByNameAsync(string name)
        => await _context.Sports.FirstOrDefaultAsync(x => x.Name == name);

    public async Task<Sport> AddAsync(Sport sport)
    {
        _context.Sports.Add(sport);
        await _context.SaveChangesAsync();
        return sport;
    }
}