using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using UrlShortener.DAL.Commands.ShortUrls.CreateShortUrl;
using UrlShortener.DAL.Database;

namespace UrlShortener.Tests.Commands;

public class CreateShortUrlCommandHandlerTests
{
    private readonly BaseDbContext _dbContext;
    private readonly Mock<ILogger<CreateShortUrlCommandHandler>> _loggerMock;
    private readonly CreateShortUrlCommandHandler _handler;

    public CreateShortUrlCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new BaseDbContext(options);
        _loggerMock = new Mock<ILogger<CreateShortUrlCommandHandler>>();
        _handler = new CreateShortUrlCommandHandler(_loggerMock.Object, _dbContext);
    }

    /// <summary>
    /// Tests that Handle method successfully creates a ShortUrl entity in the database.
    /// Verifies that:
    /// - The command handler returns a successful result
    /// - The returned value contains the correct ShortUrlCode and FullUrl
    /// - The ShortUrl is actually saved to the database with the correct values
    /// </summary>
    [Fact]
    public async Task Handle_ValidCommand_CreatesShortUrl()
    {
        var command = new CreateShortUrlCommand
        {
            FullUrl = "https://example.com",
            ShortUrlCode = "TestCode1",
            UserId = 1
        };
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("TestCode1", result.Value.ShortUrlCode);
        Assert.Equal("https://example.com", result.Value.FullUrl);
        
        var savedUrl = await _dbContext.ShortUrls.FirstOrDefaultAsync(s => s.ShortUrlCode == "TestCode1");
        Assert.NotNull(savedUrl);
        Assert.Equal("https://example.com", savedUrl.FullUrl);
    }

    /// <summary>
    /// Tests that Handle method automatically sets the CreatedAt timestamp when creating a ShortUrl.
    /// Verifies that:
    /// - The command handler successfully creates the ShortUrl
    /// - The CreatedAt field is set to a valid Instant value (not the default/empty value)
    /// </summary>
    [Fact]
    public async Task Handle_ValidCommand_SetsCreatedAt()
    {
        var command = new CreateShortUrlCommand
        {
            FullUrl = "https://example.com",
            ShortUrlCode = "TestCode2",
            UserId = 1
        };
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        Assert.True(result.IsSuccess);
        var savedUrl = await _dbContext.ShortUrls.FirstOrDefaultAsync(s => s.ShortUrlCode == "TestCode2");
        Assert.NotNull(savedUrl);
        Assert.NotEqual(default(Instant), savedUrl.CreatedAt);
    }
}

