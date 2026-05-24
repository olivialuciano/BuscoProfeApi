using BuscoProfe.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace BuscoProfe.Api.Services;

public class PendingUserRegistrationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PendingUserRegistrationCleanupService> _logger;

    public PendingUserRegistrationCleanupService(
        IServiceProvider serviceProvider,
        ILogger<PendingUserRegistrationCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await DeleteExpiredPendingRegistrationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al limpiar registraciones pendientes expiradas.");
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task DeleteExpiredPendingRegistrationsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var expirationLimit = DateTime.UtcNow.AddDays(-1);

        var expiredRegistrations = await context.PendingUserRegistrations
            .Where(x => x.CreatedAt <= expirationLimit)
            .ToListAsync(stoppingToken);

        if (expiredRegistrations.Count == 0)
            return;

        context.PendingUserRegistrations.RemoveRange(expiredRegistrations);

        await context.SaveChangesAsync(stoppingToken);

        _logger.LogInformation(
            "Se eliminaron {Count} registraciones pendientes expiradas.",
            expiredRegistrations.Count
        );
    }
}