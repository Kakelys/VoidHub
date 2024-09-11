
using ForumApi.Services.Utils.Interfaces;
using ForumApi.Utils.Extensions;
using ForumApi.Utils.Options;
using Microsoft.Extensions.Options;

namespace ForumApi.Utils.Background;

public class OnlineStatsUpdateService(
    ILogger<GarbageFileService> logger,
    IServiceScopeFactory scope,
    ISessionStorage sessionStorage,
    IOptions<UtilsOptions> utilsOptions
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = UpdateOnlineStats(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task UpdateOnlineStats(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var scp = scope.CreateAsyncScope();
            var notifyService = scp.ServiceProvider.GetService<INotifyService>();

            try
            {
                await notifyService!.NotifyAll(sessionStorage.GetOnlineStats());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            await scp.DisposeAsync();

            await Task.Delay(utilsOptions.Value.OnlineStatsUpdateDelay, CancellationToken.None);
        }
    }
}