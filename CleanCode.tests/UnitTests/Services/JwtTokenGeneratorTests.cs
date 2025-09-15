using CleanCode.Common.Security;
using CleanCode.Common.Security.Interfaces;
using CleanCode.Domain.Entities;
using CleanCode.Domain.Enum;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Bogus;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.Services
{
    /// <summary>
    /// Testes unitários para JwtTokenGenerator
    /// </summary>
    public class JwtTokenGeneratorTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly JwtTokenGenerator _generator;
        private readonly Faker<User> _userFaker;

        public JwtTokenGeneratorTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["Jwt:SecretKey"]).Returns("UOjciHkVB9c57HhsxvCWS7Mpdkpd8Pb1aeYt1exf79WrGkJE0zRY61gX5qSYL1SIwOMqExo4DoX1dUpxKXEU");
            
            _generator = new JwtTokenGenerator(_mockConfiguration.Object);

            _userFaker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(u => u.Role, f => f.PickRandom<UserRole>())
                .RuleFor(u => u.Status, f => f.PickRandom<UserStatus>())
                .RuleFor(u => u.CreatedAt, f => f.Date.Recent())
                .RuleFor(u => u.UpdatedAt, f => f.Date.Recent());
        }

        [Fact]
        public void GenerateToken_WithValidUser_ShouldReturnToken()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act
            var token = _generator.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            token.Should().Contain(".");
            token.Split('.').Should().HaveCount(3); // JWT has 3 parts
        }

        [Fact]
        public void GenerateToken_WithDifferentUsers_ShouldReturnDifferentTokens()
        {
            // Arrange
            var user1 = _userFaker.Generate();
            var user2 = _userFaker.Generate();

            // Act
            var token1 = _generator.GenerateToken(user1);
            var token2 = _generator.GenerateToken(user2);

            // Assert
            token1.Should().NotBe(token2);
        }

        [Fact]
        public void GenerateToken_WithSameUser_ShouldReturnValidTokens()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act
            var token1 = _generator.GenerateToken(user);
            var token2 = _generator.GenerateToken(user);

            // Assert
            token1.Should().NotBeNullOrEmpty();
            token2.Should().NotBeNullOrEmpty();
            token1.Split('.').Should().HaveCount(3);
            token2.Split('.').Should().HaveCount(3);
        }

        [Fact]
        public void GenerateToken_ShouldIncludeUserClaims()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act
            var token = _generator.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            // Decode JWT payload (base64)
            var parts = token.Split('.');
            parts.Should().HaveCount(3);
            
            var payload = parts[1];
            // Add padding if needed
            while (payload.Length % 4 != 0)
                payload += "=";
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
            
            payloadJson.Should().Contain(user.Id.ToString());
            payloadJson.Should().Contain(user.Username);
            payloadJson.Should().Contain(user.Email);
            payloadJson.Should().Contain(user.Role.ToString());
        }

        [Fact]
        public void GenerateToken_WithAdminUser_ShouldIncludeAdminRole()
        {
            // Arrange
            var user = _userFaker.Generate();
            user.Role = UserRole.Admin;

            // Act
            var token = _generator.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var parts = token.Split('.');
            var payload = parts[1];
            // Add padding if needed
            while (payload.Length % 4 != 0)
                payload += "=";
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
            
            payloadJson.Should().Contain("Admin");
        }

        [Fact]
        public void GenerateToken_WithManagerUser_ShouldIncludeManagerRole()
        {
            // Arrange
            var user = _userFaker.Generate();
            user.Role = UserRole.Manager;

            // Act
            var token = _generator.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var parts = token.Split('.');
            var payload = parts[1];
            // Add padding if needed
            while (payload.Length % 4 != 0)
                payload += "=";
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
            
            payloadJson.Should().Contain("Manager");
        }

        [Fact]
        public void GenerateToken_WithCustomerUser_ShouldIncludeCustomerRole()
        {
            // Arrange
            var user = _userFaker.Generate();
            user.Role = UserRole.Customer;

            // Act
            var token = _generator.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var parts = token.Split('.');
            var payload = parts[1];
            // Add padding if needed
            while (payload.Length % 4 != 0)
                payload += "=";
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
            
            payloadJson.Should().Contain("Customer");
        }

        [Fact]
        public void GenerateToken_ShouldIncludeExpirationTime()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act
            var token = _generator.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var parts = token.Split('.');
            var payload = parts[1];
            // Add padding if needed
            while (payload.Length % 4 != 0)
                payload += "=";
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
            
            payloadJson.Should().Contain("exp");
        }

        [Fact]
        public void GenerateToken_ShouldIncludeIssuedAtTime()
        {
            // Arrange
            var user = _userFaker.Generate();

            // Act
            var token = _generator.GenerateToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var parts = token.Split('.');
            var payload = parts[1];
            // Add padding if needed
            while (payload.Length % 4 != 0)
                payload += "=";
            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
            
            payloadJson.Should().Contain("iat");
        }

        [Fact]
        public void GenerateToken_WithNullSecretKey_ShouldThrowException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:SecretKey"]).Returns((string?)null);
            var generator = new JwtTokenGenerator(mockConfig.Object);
            var user = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => generator.GenerateToken(user));
            exception.ParamName.Should().Be("s");
        }

        [Fact]
        public void GenerateToken_WithEmptySecretKey_ShouldThrowException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x["Jwt:SecretKey"]).Returns("");
            var generator = new JwtTokenGenerator(mockConfig.Object);
            var user = _userFaker.Generate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => generator.GenerateToken(user));
            exception.Message.Should().NotBeNullOrEmpty();
        }
    }
}
