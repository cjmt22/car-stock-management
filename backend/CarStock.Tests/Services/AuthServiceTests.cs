using CarStock.API.DTOs;
using CarStock.API.Services;
using CarStock.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace CarStock.Tests.Services
{
    public class AuthServiceTests
    {
        // Helper: creates a mock IConfiguration with JWT settings
        // We need this because AuthService reads JWT settings from config
        private static IConfiguration GetTestConfig()
        {
            var config = new Mock<IConfiguration>();

            config.Setup(c => c["Jwt:Key"])
                .Returns("TestSuperSecretKeyThatIsAtLeast32Chars!");
            config.Setup(c => c["Jwt:Issuer"])
                .Returns("TestIssuer");
            config.Setup(c => c["Jwt:Audience"])
                .Returns("TestAudience");
            config.Setup(c => c["Jwt:ExpiresInMinutes"])
                .Returns("60");

            return config.Object;
        }

        // ─────────────────────────────────────────
        // Register Tests
        // ─────────────────────────────────────────

        [Fact]
        public async Task Register_WithValidData_ReturnsToken()
        {
            // Arrange — set up the test
            var db = TestDbContextFactory.Create();
            var service = new AuthService(db, GetTestConfig());

            var dto = new RegisterDto
            {
                Name = "Carl Tungul",
                Email = "carl@test.com",
                Password = "password123"
            };

            // Act — run the thing we're testing
            var result = await service.RegisterAsync(dto);

            // Assert — check the result is what we expect
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Name.Should().Be("Carl Tungul");
            result.Role.Should().Be("Buyer");
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ReturnsNull()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            var service = new AuthService(db, GetTestConfig());

            var dto = new RegisterDto
            {
                Name = "Another User",
                Email = "buyer@test.com",  // already exists in seeded data
                Password = "password123"
            };

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert — should return null because email is taken
            result.Should().BeNull();
        }

        // ─────────────────────────────────────────
        // Login Tests
        // ─────────────────────────────────────────

        [Fact]
        public async Task Login_WithCorrectCredentials_ReturnsToken()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            var service = new AuthService(db, GetTestConfig());

            var dto = new LoginDto
            {
                Email = "buyer@test.com",
                Password = "password123"
            };

            // Act
            var result = await service.LoginAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("buyer@test.com");
        }

        [Fact]
        public async Task Login_WithWrongPassword_ReturnsNull()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);
            var service = new AuthService(db, GetTestConfig());

            var dto = new LoginDto
            {
                Email = "buyer@test.com",
                Password = "wrongpassword"
            };

            // Act
            var result = await service.LoginAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_WithNonExistentEmail_ReturnsNull()
        {
            // Arrange
            var db = TestDbContextFactory.Create();
            var service = new AuthService(db, GetTestConfig());

            var dto = new LoginDto
            {
                Email = "nobody@test.com",
                Password = "password123"
            };

            // Act
            var result = await service.LoginAsync(dto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_WithInactiveAccount_ReturnsNull()
        {
            // Arrange — create an inactive user
            var db = TestDbContextFactory.Create();
            await TestDataSeeder.SeedUsersAsync(db);

            // Mark buyer as inactive
            var user = db.Users.Find(3);
            user!.IsActive = false;
            await db.SaveChangesAsync();

            var service = new AuthService(db, GetTestConfig());

            var dto = new LoginDto
            {
                Email = "buyer@test.com",
                Password = "password123"
            };

            // Act
            var result = await service.LoginAsync(dto);

            // Assert — inactive accounts can't login
            result.Should().BeNull();
        }
    }
}