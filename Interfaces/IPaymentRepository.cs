using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<List<Payment>> GetByMembershipIdAsync(int membershipId);
    Task<Payment> AddAsync(Payment entity);
    Task UpdateAsync(Payment entity);
}