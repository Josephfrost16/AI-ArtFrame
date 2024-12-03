using Xunit;
using Microsoft.EntityFrameworkCore;
using login.BDD;
using login.Servicios;
using System;
using System.Threading.Tasks;

namespace Ai_ArtFrame_Test
{
    public class UserServiceTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Register_Should_RegisterUser_When_ValidData()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();
            var userService = new UserService(dbContext);

            // Act
            await userService.Register("Test User", "test@gmail.com", "password123");

            // Assert
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "test@gmail.com");
            Assert.NotNull(user);
            Assert.Equal("Test User", user!.Name);
        }

        [Fact]
        public async Task Register_Should_ThrowException_When_EmailAlreadyExists()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();
            var userService = new UserService(dbContext);

            // Seed an existing user
            await userService.Register("Existing User", "test@gmail.com", "password123");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                userService.Register("New User", "test@gmail.com", "password123"));
            Assert.Equal("El correo ya está registrado.", exception.Message);
        }

        [Fact]
        public async Task Register_Should_ThrowException_When_EmailDomainNotAllowed()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();
            var userService = new UserService(dbContext);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() =>
                userService.Register("Test User", "test@invalid.com", "password123"));
            Assert.Equal("El dominio del correo no está permitido.", exception.Message);
        }

        [Fact]
        public async Task Login_Should_ReturnUser_When_ValidCredentials()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();
            var userService = new UserService(dbContext);

            // Seed a user
            await userService.Register("Test User", "test@gmail.com", "password123");

            // Act
            var user = await userService.Login("test@gmail.com", "password123");

            // Assert
            Assert.NotNull(user);
            Assert.Equal("Test User", user!.Name);
        }

        [Fact]
        public async Task Login_Should_ReturnNull_When_EmailDoesNotExist()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();
            var userService = new UserService(dbContext);

            // Act
            var user = await userService.Login("nonexistent@gmail.com", "password123");

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task Login_Should_ReturnNull_When_PasswordIsIncorrect()
        {
            // Arrange
            var dbContext = CreateInMemoryDbContext();
            var userService = new UserService(dbContext);

            // Seed a user
            await userService.Register("Test User", "test@gmail.com", "password123");

            // Act
            var user = await userService.Login("test@gmail.com", "wrongpassword");

            // Assert
            Assert.Null(user);
        }
    }
}