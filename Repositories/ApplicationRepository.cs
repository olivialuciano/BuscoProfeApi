using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly AppDbContext _context;

    public ApplicationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Application>> GetAllAsync()
        => await _context.Applications
            .Include(x => x.JobPosting)
            .Include(x => x.ProfessorUser)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync();

    public async Task<List<Application>> GetByProfessorUserIdAsync(int professorUserId)
        => await _context.Applications
            .Include(x => x.JobPosting)
            .Where(x => x.ProfessorUserId == professorUserId)
            .OrderByDescending(x => x.AppliedAt)
            .ToListAsync();

    public async Task<Application?> GetByIdAsync2(int id)
    {
        return await _context.Applications
            .Include(x => x.ProfessorUser)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Application?> GetByIdAsync(int id)
        => await _context.Applications
            .Include(x => x.JobPosting)
            .Include(x => x.ProfessorUser)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Application?> GetByJobPostingAndProfessorAsync(int jobPostingId, int professorUserId)
        => await _context.Applications
            .FirstOrDefaultAsync(x => x.JobPostingId == jobPostingId && x.ProfessorUserId == professorUserId);

    public async Task<Application> AddAsync(Application entity)
    {
        _context.Applications.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Application entity)
    {
        _context.Applications.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task AcceptAsync(Application application)
    {
        application.Status = ApplicationStatus.Aceptado;
        application.UpdatedAt = DateTime.UtcNow;

        _context.Applications.Update(application);
        await _context.SaveChangesAsync();
    }

    public async Task RejectAsync(Application application)
    {
        application.Status = ApplicationStatus.Rechazado;
        application.UpdatedAt = DateTime.UtcNow;

        _context.Applications.Update(application);
        await _context.SaveChangesAsync();
    }
}