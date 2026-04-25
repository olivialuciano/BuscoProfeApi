using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class JobPostingRepository : IJobPostingRepository
{
    private readonly AppDbContext _context;

    public JobPostingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobPosting>> GetAllAsync()
        => await _context.JobPostings
            .Include(x => x.InstitutionUser)
            .Include(x => x.Sport)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<List<JobPosting>> GetByInstitutionUserIdAsync(int institutionUserId)
        => await _context.JobPostings
            .Include(x => x.Sport)
            .Where(x => x.InstitutionUserId == institutionUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<JobPosting?> GetByIdAsync(int id)
        => await _context.JobPostings
            .Include(x => x.InstitutionUser)
            .Include(x => x.Sport)
            .Include(x => x.Applications)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<JobPosting> AddAsync(JobPosting entity)
    {
        _context.JobPostings.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(JobPosting entity)
    {
        _context.JobPostings.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> InstitutionHasAnyAsync(int institutionUserId)
    {
        return await _context.JobPostings.AnyAsync(x => x.InstitutionUserId == institutionUserId);
    }
}