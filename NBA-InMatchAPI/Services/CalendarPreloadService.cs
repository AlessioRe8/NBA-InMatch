namespace progetto_cloud.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class CalendarPreloadService : IHostedService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<CalendarPreloadService> _logger;

    public CalendarPreloadService(IServiceProvider provider,
                                  ILogger<CalendarPreloadService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Preloading 2024 calendar into cache...");
        try
        {
            // Creo uno scope per consumare servizi scoped
            using var scope = _provider.CreateScope();
            var gameService = scope.ServiceProvider.GetRequiredService<IGameService>();

            // Chiama il metodo con caching interno
            await gameService.GetAllGamesBySeasonDescendingAsync(2024);
            _logger.LogInformation("Preloading complete.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during preload of 2024 calendar");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}