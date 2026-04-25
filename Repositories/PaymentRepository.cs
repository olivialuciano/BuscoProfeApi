using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int id)
        => await _context.Payments
            .Include(x => x.Membership)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<List<Payment>> GetByMembershipIdAsync(int membershipId)
        => await _context.Payments
            .Where(x => x.MembershipId == membershipId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<Payment> AddAsync(Payment entity)
    {
        _context.Payments.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Payment entity)
    {
        _context.Payments.Update(entity);
        await _context.SaveChangesAsync();
    }
}