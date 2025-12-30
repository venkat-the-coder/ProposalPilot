using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ProposalPilot.Tests.Integration.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Register_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        // The response has properties directly (userId, email, accessToken, etc.)
        root.TryGetProperty("userId", out _).Should().BeTrue("response should contain userId");
        root.TryGetProperty("email", out _).Should().BeTrue("response should contain email");
        root.TryGetProperty("accessToken", out var accessToken).Should().BeTrue("response should contain accessToken");
        root.TryGetProperty("refreshToken", out _).Should().BeTrue("response should contain refreshToken");
        accessToken.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new
        {
            Email = "invalid-email",
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange - First register a user with a fresh client to avoid auth header issues
        using var registerClient = _factory.CreateClient();
        var email = $"login{Guid.NewGuid()}@example.com";
        var password = "Password123!";

        var registerResponse = await registerClient.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = password,
            FirstName = "Test",
            LastName = "User"
        });
        registerResponse.EnsureSuccessStatusCode();

        // Act - Login with same credentials using a fresh client
        using var loginClient = _factory.CreateClient();
        var response = await loginClient.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        root.TryGetProperty("accessToken", out var accessToken).Should().BeTrue();
        accessToken.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Arrange - First register a user
        using var registerClient = _factory.CreateClient();
        var email = $"wrongpass{Guid.NewGuid()}@example.com";

        await registerClient.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = "CorrectPassword123!",
            FirstName = "Test",
            LastName = "User"
        });

        // Act - Try to login with wrong password using fresh client
        using var loginClient = _factory.CreateClient();
        var response = await loginClient.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = "WrongPassword123!"
        });

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.Unauthorized, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Me_WithValidToken_ReturnsUserInfo()
    {
        // Arrange - Register and get token
        using var registerClient = _factory.CreateClient();
        var email = $"me{Guid.NewGuid()}@example.com";
        var registerResponse = await registerClient.PostAsJsonAsync("/api/auth/register", new
        {
            Email = email,
            Password = "Password123!",
            FirstName = "Test",
            LastName = "User"
        });

        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(registerContent);
        var accessToken = doc.RootElement.GetProperty("accessToken").GetString();

        // Act - Use fresh client with the token
        using var meClient = _factory.CreateClient();
        meClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await meClient.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Me_WithoutToken_ReturnsUnauthorized()
    {
        // Arrange - Use a fresh client with no auth header
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
