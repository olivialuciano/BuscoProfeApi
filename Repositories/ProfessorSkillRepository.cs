using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class ProfessorSkillRepository : IProfessorSkillRepository
{
    private readonly AppDbContext _context;

    public ProfessorSkillRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfessorSkill>> GetByUserIdAsync(int userId)
        => await _context.ProfessorSkills
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<ProfessorSkill?> GetByIdAsync(int id)
        => await _context.ProfessorSkills.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ProfessorSkill> AddAsync(ProfessorSkill entity)
    {
        _context.ProfessorSkills.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(ProfessorSkill entity)
    {
        _context.ProfessorSkills.Remove(entity);
        await _context.SaveChangesAsync();
    }
}