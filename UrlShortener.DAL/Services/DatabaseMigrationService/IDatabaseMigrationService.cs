namespace UrlShortener.DAL.Services.DatabaseMigrationService;

public interface IDatabaseMigrationService
{
    Task MigrateAsync();
}