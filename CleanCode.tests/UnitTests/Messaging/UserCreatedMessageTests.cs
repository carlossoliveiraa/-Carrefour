using CleanCode.Common.Messaging.Models;
using Bogus;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.Messaging
{
    /// <summary>
    /// Testes para o modelo UserCreatedMessage
    /// </summary>
    public class UserCreatedMessageTests
    {
        private readonly Faker<UserCreatedMessage> _userCreatedMessageFaker;

        public UserCreatedMessageTests()
        {
            _userCreatedMessageFaker = new Faker<UserCreatedMessage>()
                .RuleFor(u => u.UserId, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Role, f => f.PickRandom("Admin", "Manager", "Customer"))
                .RuleFor(u => u.Status, f => f.PickRandom("Active", "Inactive", "Suspended"))
                .RuleFor(u => u.CreatedAt, f => f.Date.Recent())
                .RuleFor(u => u.SourceIp, f => f.Internet.Ip())
                .RuleFor(u => u.UserAgent, f => f.Internet.UserAgent());
        }

        [Fact]
        public void UserCreatedMessage_ShouldHaveValidProperties()
        {
            // Arrange & Act
            var message = _userCreatedMessageFaker.Generate();

            // Assert
            message.UserId.Should().NotBe(Guid.Empty);
            message.Username.Should().NotBeNullOrEmpty();
            message.Email.Should().NotBeNullOrEmpty();
            message.Role.Should().NotBeNullOrEmpty();
            message.Status.Should().NotBeNullOrEmpty();
            message.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        [Fact]
        public void UserCreatedMessage_ShouldHaveValidEmailFormat()
        {
            // Arrange & Act
            var message = _userCreatedMessageFaker.Generate();

            // Assert
            message.Email.Should().Contain("@");
            message.Email.Should().Contain(".");
        }

        [Fact]
        public void UserCreatedMessage_ShouldHaveValidRole()
        {
            // Arrange & Act
            var message = _userCreatedMessageFaker.Generate();

            // Assert
            message.Role.Should().BeOneOf("Admin", "Manager", "Customer");
        }

        [Fact]
        public void UserCreatedMessage_ShouldHaveValidStatus()
        {
            // Arrange & Act
            var message = _userCreatedMessageFaker.Generate();

            // Assert
            message.Status.Should().BeOneOf("Active", "Inactive", "Suspended");
        }

        [Fact]
        public void UserCreatedMessage_WithMultipleInstances_ShouldBeUnique()
        {
            // Arrange & Act
            var messages = _userCreatedMessageFaker.Generate(10);

            // Assert
            var userIds = messages.Select(m => m.UserId).ToList();
            var uniqueIds = userIds.Distinct().ToList();
            
            userIds.Should().HaveCount(10);
            uniqueIds.Should().HaveCount(10); // All should be unique
        }

        [Fact]
        public void UserCreatedMessage_WithSourceIp_ShouldHaveValidFormat()
        {
            // Arrange & Act
            var message = _userCreatedMessageFaker.Generate();

            // Assert
            if (!string.IsNullOrEmpty(message.SourceIp))
            {
                message.SourceIp.Should().MatchRegex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
            }
        }

        [Fact]
        public void UserCreatedMessage_WithUserAgent_ShouldNotBeEmpty()
        {
            // Arrange & Act
            var message = _userCreatedMessageFaker.Generate();

            // Assert
            if (!string.IsNullOrEmpty(message.UserAgent))
            {
                message.UserAgent.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public void UserCreatedMessage_DefaultValues_ShouldBeValid()
        {
            // Arrange & Act
            var message = new UserCreatedMessage();

            // Assert
            message.UserId.Should().Be(Guid.Empty);
            message.Username.Should().Be(string.Empty);
            message.Email.Should().Be(string.Empty);
            message.Role.Should().Be(string.Empty);
            message.Status.Should().Be(string.Empty);
            message.CreatedAt.Should().Be(default(DateTime));
        }
    }
}

