using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Gen.Backend.Tests;

using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UsersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    /*private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly AppDbContext _context;

    public UsersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // WebApplicationFactory konfigurieren
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Entfernen des bestehenden DbContext-Providers (PostgreSQL)
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // In-Memory-Datenbank nur für Tests verwenden
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });
            });
        });

        _client = _factory.CreateClient(); // HttpClient für API-Tests

        // Wir erstellen einen neuen Scope, um auf den DbContext zuzugreifen
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }

    [Fact]
    public async Task GetUsers_ReturnsAllUsers()
    {
        // Arrange: Füge Testdaten hinzu
        _context.Users.Add(new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Password = "123" });
        _context.Users.Add(new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Password = "456" });
        await _context.SaveChangesAsync();

        // Act: HTTP GET request an den Endpoint
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.EnsureSuccessStatusCode();  // Status-Code ist 2xx
        var users = await response.Content.ReadFromJsonAsync<List<User>>();
        Assert.Equal(2, users.Count);  // Es gibt zwei Benutzer
    }

    [Fact]
    public async Task PostUser_AddsUserToDatabase()
    {
        // Arrange: Neuer Benutzer
        var newUser = new CreateOrUpdateUserDto
        {
            FirstName = "Alice",
            LastName = "Wonderland",
            Email = "alice@example.com",
            Password = "789"
        };

        // Act: HTTP POST request an den Endpoint
        var response = await _client.PostAsJsonAsync("/api/users", newUser);

        // Assert
        response.EnsureSuccessStatusCode(); // Status-Code ist 2xx
        var createdUser = await response.Content.ReadFromJsonAsync<User>();
        Assert.Equal("Alice", createdUser.FirstName);

        // Verifiziere, dass der Benutzer in der Datenbank ist
        var userInDb = await _context.Users.FindAsync(createdUser.Id);
        Assert.NotNull(userInDb);
        Assert.Equal("Alice", userInDb.FirstName);
    }

    [Fact]
    public async Task DeleteUser_RemovesUserFromDatabase()
    {
        // Arrange: Füge einen Benutzer hinzu, den wir löschen können
        var user = new User { FirstName = "John", LastName = "Doe", Email = "john@example.com", Password = "123" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act: HTTP DELETE request an den Endpoint
        var response = await _client.DeleteAsync($"/api/users/{user.Id}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status-Code ist 2xx
        var userInDb = await _context.Users.FindAsync(user.Id);
        Assert.Null(userInDb); // Benutzer sollte nicht mehr in der DB sein
    }*/
}

