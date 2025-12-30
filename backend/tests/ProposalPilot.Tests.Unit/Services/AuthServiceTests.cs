using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProposalPilot.Domain.Entities;
using ProposalPilot.Infrastructure.Data;
using ProposalPilot.Infrastructure.Services;
using ProposalPilot.Shared.Configuration;

namespace ProposalPilot.Tests.Unit.Services;

public class AuthServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);

        var jwtSettings = new JwtSettings
        {
            Secret = "ThisIsAVeryLongSecretKeyForTesting12345678901234567890",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationInMinutes = 60,
            RefreshTokenExpirationInDays = 7
        };

        var jwtOptions = Options.Create(jwtSettings);
        _authService = new AuthService(_context, jwtOptions);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_ShouldCreateUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        var firstName = "Test";
        var lastName = "User";

        // Act
        var (user, accessToken, refreshToken) = await _authService.RegisterAsync(email, password, firstName, lastName);

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be(email.ToLower());
        accessToken.Should().NotBeNullOrEmpty();
        refreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ShouldThrowException()
    {
        // Arrange
        var email = "duplicate@example.com";

        // First registration
        await _authService.RegisterAsync(email, "Password123!", "First", "User");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _authService.RegisterAsync(email, "Password456!", "Second", "User"));
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var email = "login@example.com";
        var password = "Password123!";
        await _authService.RegisterAsync(email, password, "Test", "User");

        // Act
        var result = await _authService.LoginAsync(email, password);

        // Assert
        result.Should().NotBeNull();
        result!.Value.user.Email.Should().Be(email.ToLower());
        result.Value.accessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var email = "login@example.com";
        await _authService.RegisterAsync(email, "CorrectPassword123!", "Test", "User");

        // Act
        var result = await _authService.LoginAsync(email, "WrongPassword123!");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentUser_ShouldReturnNull()
    {
        // Act
        var result = await _authService.LoginAsync("nonexistent@example.com", "Password123!");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        var (_, _, refreshToken) = await _authService.RegisterAsync(
            "refresh@example.com", "Password123!", "Test", "User");

        // Act
        var result = await _authService.RefreshTokenAsync(refreshToken);

        // Assert
        result.Should().NotBeNull();
        result!.Value.accessToken.Should().NotBeNullOrEmpty();
        result.Value.refreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshTokenAsync_WithInvalidToken_ShouldReturnNull()
    {
        // Act
        var result = await _authService.RefreshTokenAsync("invalid-refresh-token");

        // Assert
        result.Should().BeNull();
    }
}
