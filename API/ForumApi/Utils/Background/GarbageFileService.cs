using ForumApi.Data.Repository.Interfaces;
using ForumApi.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ForumApi.Utils.Background
{
    public class GarbageFileService(
        ILogger<GarbageFileService> logger,
        IServiceScopeFactory scope,
        IOptions<ImageOptions> imgOptions) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = DeleteNotLinkedFiles(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task DeleteNotLinkedFiles(CancellationToken cancellationToken)
        {
            while(!cancellationToken.IsCancellationRequested)
            {
                var scp = scope.CreateAsyncScope();
                var rep = scp.ServiceProvider.GetService<IRepositoryManager>();

                try
                {
                    var garbageFiles = await rep!.File.Value
                        .FindByCondition(f => f.CreatedAt.AddDays(7) < DateTime.UtcNow && f.PostId == null)
                        .ToListAsync(CancellationToken.None);

                    rep.File.Value.DeleteMany(garbageFiles);
                    await rep.Save();

                    logger.LogInformation($"Deleted garbage files: {garbageFiles.Count}");
                }
                catch(Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                }

                await scp.DisposeAsync();

                await Task.Delay(imgOptions.Value.GarbageFileDeleteDelay, CancellationToken.None);
            }
        }
    }
}