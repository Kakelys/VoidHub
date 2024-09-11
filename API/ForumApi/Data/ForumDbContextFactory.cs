using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ForumApi.Data;

/// <summary>
/// used by dotnet ef
/// </summary>
public class ForumDbContextFactory : IDesignTimeDbContextFactory<ForumDbContext>
{
    public ForumDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ForumDbContext>();

        string workingDirectory = Environment.CurrentDirectory;

        var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile($"{workingDirectory}\\appsettings.json")
        .AddUserSecrets<Program>();

        IConfigurationRoot config = builder.Build();

        string connectionString = config.GetConnectionString("ForumDb");
        optionsBuilder.UseNpgsql(connectionString, opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(10).TotalSeconds));

        return new ForumDbContext(optionsBuilder.Options);
    }
}