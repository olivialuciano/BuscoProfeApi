using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class FavoriteInstitutionRepository : IFavoriteInstitutionRepository
{
    private readonly AppDbContext _context;

    public FavoriteInstitutionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FavoriteInstitution>> GetByProfessorUserIdAsync(int professorUserId)
    {
        return await _context.FavoriteInstitutions
            .Include(x => x.InstitutionUser)
            .Where(x => x.ProfessorUserId == professorUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<FavoriteInstitution?> GetByIdAsync(int id)
    {
        return await _context.FavoriteInstitutions
            .Include(x => x.InstitutionUser)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FavoriteInstitution?> GetByProfessorAndInstitutionAsync(int professorUserId, int institutionUserId)
    {
        return await _context.FavoriteInstitutions
            .FirstOrDefaultAsync(x => x.ProfessorUserId == professorUserId && x.InstitutionUserId == institutionUserId);
    }

    public async Task<FavoriteInstitution> AddAsync(FavoriteInstitution entity)
    {
        _context.FavoriteInstitutions.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<FavoriteInstitution>> GetByInstitutionUserIdAsync(int institutionUserId)
    {
        return await _context.FavoriteInstitutions
            .Where(x => x.InstitutionUserId == institutionUserId)
            .ToListAsync();
    }
    public async Task DeleteAsync(FavoriteInstitution entity)
    {
        _context.FavoriteInstitutions.Remove(entity);
        await _context.SaveChangesAsync();
    }
}