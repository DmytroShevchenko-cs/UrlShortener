namespace UrlShortener.DAL.Services.DatabaseMigrationService;

using Microsoft.EntityFrameworkCore;
using Database;

public class DatabaseMigrationService : IDatabaseMigrationService
{
    private readonly BaseDbContext _dbContext;

    public DatabaseMigrationService(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task MigrateAsync()
    {
        await _dbContext.Database.MigrateAsync();
    }
}