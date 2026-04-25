using BuscoProfe.Api.Entities;

namespace BuscoProfe.Api.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    DateTime GetExpirationUtc();
}