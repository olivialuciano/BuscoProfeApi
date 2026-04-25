using BuscoProfe.Api.Data;
using BuscoProfe.Api.Entities;
using BuscoProfe.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> AddAsync(Notification entity)
    {
        _context.Notifications.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
        => await _context.Notifications
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

    public async Task<Notification?> GetByIdAsync(int id)
        => await _context.Notifications.FirstOrDefaultAsync(x => x.Id == id);

    public async Task UpdateAsync(Notification entity)
    {
        _context.Notifications.Update(entity);
        await _context.SaveChangesAsync();
    }
}