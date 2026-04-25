using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class ProfessorCertificationRepository : IProfessorCertificationRepository
{
    private readonly AppDbContext _context;

    public ProfessorCertificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfessorCertification>> GetByUserIdAsync(int userId)
        => await _context.ProfessorCertifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<ProfessorCertification?> GetByIdAsync(int id)
        => await _context.ProfessorCertifications.FirstOrDefaultAsync(x => x.Id == id);

    public async Task<ProfessorCertification> AddAsync(ProfessorCertification entity)
    {
        _context.ProfessorCertifications.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(ProfessorCertification entity)
    {
        _context.ProfessorCertifications.Remove(entity);
        await _context.SaveChangesAsync();
    }
}