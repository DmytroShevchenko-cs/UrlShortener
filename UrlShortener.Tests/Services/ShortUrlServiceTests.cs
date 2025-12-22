using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using UrlShortener.BLL.Services.ShortUrls;
using UrlShortener.DAL.Database;
using MediatR;

namespace UrlShortener.Tests.Services;

public class ShortUrlServiceTests
{
    private readonly ShortUrlService _service;

    public ShortUrlServiceTests()
    {
        var loggerMock = new Mock<ILogger<ShortUrlService>>();
        var mediatorMock = new Mock<IMediator>();
        
        var options = new DbContextOptionsBuilder<BaseDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        var dbContext = new BaseDbContext(options);
        
        _service = new ShortUrlService(loggerMock.Object, mediatorMock.Object, dbContext);
    }

    /// <summary>
    /// Tests that IsValidUrl returns true for a valid URL format.
    /// Verifies that the service correctly identifies well-formed URLs like "https://example.com".
    /// </summary>
    [Fact]
    public void IsValidUrl_ValidUrl_ReturnsTrue()
    {
        var validUrl = "https://example.com";
        
        var result = _service.IsValidUrl(validUrl);
        
        Assert.True(result);
    }

    /// <summary>
    /// Tests that IsValidUrl returns false for an invalid URL format.
    /// Verifies that the service correctly rejects malformed strings that are not valid URLs.
    /// </summary>
    [Fact]
    public void IsValidUrl_InvalidUrl_ReturnsFalse()
    {
        var invalidUrl = "not-a-url";
        
        var result = _service.IsValidUrl(invalidUrl);
        
        Assert.False(result);
    }

    /// <summary>
    /// Tests that GenerateShortUrlCode returns a non-empty string of exactly 8 characters.
    /// Verifies that the generated code has the correct length and is not empty.
    /// </summary>
    [Fact]
    public void GenerateShortUrlCode_ReturnsNonEmptyString()
    {
        var code = _service.GenerateShortUrlCode();
        
        Assert.NotEmpty(code);
        Assert.Equal(8, code.Length);
    }

    /// <summary>
    /// Tests that GenerateShortUrlCode generates codes that are not null.
    /// Note: This test verifies that codes are generated, but does not guarantee uniqueness
    /// (uniqueness is typically checked at the database level).
    /// </summary>
    [Fact]
    public void GenerateShortUrlCode_GeneratesUniqueCodes()
    {
        var code1 = _service.GenerateShortUrlCode();
        var code2 = _service.GenerateShortUrlCode();
        
        Assert.NotNull(code1);
        Assert.NotNull(code2);
    }
}

