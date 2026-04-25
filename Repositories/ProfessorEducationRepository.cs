using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class ProfessorEducationRepository : IProfessorEducationRepository
{
    private readonly AppDbContext _context;

    public ProfessorEducationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfessorEducation>> GetByUserIdAsync(int userId)
        => await _context.ProfessorEducations
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<ProfessorEducation?> GetByIdAsync(int id)
        => await _context.ProfessorEducations.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ProfessorEducation> AddAsync(ProfessorEducation entity)
    {
        _context.ProfessorEducations.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(ProfessorEducation entity)
    {
        _context.ProfessorEducations.Remove(entity);
        await _context.SaveChangesAsync();
    }
}