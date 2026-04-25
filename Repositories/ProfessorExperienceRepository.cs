using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class ProfessorExperienceRepository : IProfessorExperienceRepository
{
    private readonly AppDbContext _context;

    public ProfessorExperienceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfessorExperience>> GetByUserIdAsync(int userId)
        => await _context.ProfessorExperiences
            .Include(x => x.Sport)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<ProfessorExperience?> GetByIdAsync(int id)
        => await _context.ProfessorExperiences.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ProfessorExperience> AddAsync(ProfessorExperience entity)
    {
        _context.ProfessorExperiences.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(ProfessorExperience entity)
    {
        _context.ProfessorExperiences.Remove(entity);
        await _context.SaveChangesAsync();
    }
}