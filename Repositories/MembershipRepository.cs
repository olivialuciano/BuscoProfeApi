using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Enums;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class MembershipRepository : IMembershipRepository
{
    private readonly AppDbContext _context;

    public MembershipRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Membership?> GetByIdAsync(int id)
        => await _context.Memberships
            .Include(x => x.Payments)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<Membership?> GetActiveByInstitutionUserIdAsync(int institutionUserId)
        => await _context.Memberships
            .Where(x => x.InstitutionUserId == institutionUserId && x.Status == MembershipStatus.Activo)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

    public async Task<List<Membership>> GetByInstitutionUserIdAsync(int institutionUserId)
        => await _context.Memberships
            .Where(x => x.InstitutionUserId == institutionUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<Membership> AddAsync(Membership entity)
    {
        _context.Memberships.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Membership entity)
    {
        _context.Memberships.Update(entity);
        await _context.SaveChangesAsync();
    }
}