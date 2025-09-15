using CleanCode.Common.Messaging.Models;
using Bogus;
using FluentAssertions;

namespace CleanCode.Tests.UnitTests.Messaging
{
    /// <summary>
    /// Testes para o modelo UserAccessedMessage
    /// </summary>
    public class UserAccessedMessageTests
    {
        private readonly Faker<UserAccessedMessage> _userAccessedMessageFaker;

        public UserAccessedMessageTests()
        {
            _userAccessedMessageFaker = new Faker<UserAccessedMessage>()
                .RuleFor(u => u.UserId, f => f.Random.Guid())
                .RuleFor(u => u.Username, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.AccessType, f => f.PickRandom("GET", "PUT", "DELETE", "PATCH"))
                .RuleFor(u => u.AccessedAt, f => f.Date.Recent())
                .RuleFor(u => u.SourceIp, f => f.Internet.Ip())
                .RuleFor(u => u.UserAgent, f => f.Internet.UserAgent())
                .RuleFor(u => u.RequestedBy, f => f.Random.Guid());
        }

        [Fact]
        public void UserAccessedMessage_ShouldHaveValidProperties()
        {
            // Arrange & Act
            var message = _userAccessedMessageFaker.Generate();

            // Assert
            message.UserId.Should().NotBe(Guid.Empty);
            message.Username.Should().NotBeNullOrEmpty();
            message.Email.Should().NotBeNullOrEmpty();
            message.AccessType.Should().NotBeNullOrEmpty();
            message.AccessedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        [Fact]
        public void UserAccessedMessage_ShouldHaveValidAccessType()
        {
            // Arrange & Act
            var message = _userAccessedMessageFaker.Generate();

            // Assert
            message.AccessType.Should().BeOneOf("GET", "PUT", "DELETE", "PATCH");
        }

        [Fact]
        public void UserAccessedMessage_ShouldHaveValidEmailFormat()
        {
            // Arrange & Act
            var message = _userAccessedMessageFaker.Generate();

            // Assert
            message.Email.Should().Contain("@");
            message.Email.Should().Contain(".");
        }

        [Fact]
        public void UserAccessedMessage_WithRequestedBy_ShouldHaveValidGuid()
        {
            // Arrange & Act
            var message = _userAccessedMessageFaker.Generate();

            // Assert
            if (message.RequestedBy.HasValue)
            {
                message.RequestedBy.Value.Should().NotBe(Guid.Empty);
            }
        }

        [Fact]
        public void UserAccessedMessage_AccessedAt_ShouldBeRecent()
        {
            // Arrange & Act
            var message = _userAccessedMessageFaker.Generate();

            // Assert
            var timeDifference = DateTime.UtcNow - message.AccessedAt;
            timeDifference.TotalDays.Should().BeLessThan(2); // Should be within last 2 days
        }

        [Fact]
        public void UserAccessedMessage_WithSourceIp_ShouldHaveValidFormat()
        {
            // Arrange & Act
            var message = _userAccessedMessageFaker.Generate();

            // Assert
            if (!string.IsNullOrEmpty(message.SourceIp))
            {
                message.SourceIp.Should().MatchRegex(@"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$");
            }
        }

        [Fact]
        public void UserAccessedMessage_WithUserAgent_ShouldNotBeEmpty()
        {
            // Arrange & Act
            var message = _userAccessedMessageFaker.Generate();

            // Assert
            if (!string.IsNullOrEmpty(message.UserAgent))
            {
                message.UserAgent.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public void UserAccessedMessage_DefaultValues_ShouldBeValid()
        {
            // Arrange & Act
            var message = new UserAccessedMessage();

            // Assert
            message.UserId.Should().Be(Guid.Empty);
            message.Username.Should().Be(string.Empty);
            message.Email.Should().Be(string.Empty);
            message.AccessType.Should().Be(string.Empty);
            message.AccessedAt.Should().Be(default(DateTime));
            message.RequestedBy.Should().BeNull();
        }

        [Fact]
        public void UserAccessedMessage_WithMultipleInstances_ShouldBeUnique()
        {
            // Arrange & Act
            var messages = _userAccessedMessageFaker.Generate(10);

            // Assert
            var userIds = messages.Select(m => m.UserId).ToList();
            var uniqueIds = userIds.Distinct().ToList();
            
            userIds.Should().HaveCount(10);
            uniqueIds.Should().HaveCount(10); // All should be unique
        }
    }
}

