using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface INotificationRepository
{
    Task<Notification> AddAsync(Notification entity);
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<Notification?> GetByIdAsync(int id);
    Task UpdateAsync(Notification entity);
}