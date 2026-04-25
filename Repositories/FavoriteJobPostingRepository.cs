using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class FavoriteJobPostingRepository : IFavoriteJobPostingRepository
{
    private readonly AppDbContext _context;

    public FavoriteJobPostingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FavoriteJobPosting>> GetByProfessorUserIdAsync(int professorUserId)
    {
        return await _context.FavoriteJobPostings
            .Include(x => x.JobPosting)
                .ThenInclude(j => j.InstitutionUser)
            .Include(x => x.JobPosting)
                .ThenInclude(j => j.Sport)
            .Where(x => x.ProfessorUserId == professorUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<FavoriteJobPosting?> GetByIdAsync(int id)
    {
        return await _context.FavoriteJobPostings
            .Include(x => x.JobPosting)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FavoriteJobPosting?> GetByProfessorAndJobPostingAsync(int professorUserId, int jobPostingId)
    {
        return await _context.FavoriteJobPostings
            .FirstOrDefaultAsync(x => x.ProfessorUserId == professorUserId && x.JobPostingId == jobPostingId);
    }

    public async Task<FavoriteJobPosting> AddAsync(FavoriteJobPosting entity)
    {
        _context.FavoriteJobPostings.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(FavoriteJobPosting entity)
    {
        _context.FavoriteJobPostings.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByJobPostingIdAsync(int jobPostingId)
    {
        var items = await _context.FavoriteJobPostings
            .Where(x => x.JobPostingId == jobPostingId)
            .ToListAsync();

        if (items.Count == 0)
            return;

        _context.FavoriteJobPostings.RemoveRange(items);
        await _context.SaveChangesAsync();
    }
}