using CarlosAOliveira.Developer.Domain.Entities;
using CarlosAOliveira.Developer.ORM.Repositories;
using CarlosAOliveira.Developer.Tests.Builders;
using CarlosAOliveira.Developer.Tests.Infrastructure.Common;
using FluentAssertions;

namespace CarlosAOliveira.Developer.Tests.Infrastructure.Repositories
{
    /// <summary>
    /// Unit tests for BaseRepository
    /// </summary>
    public class BaseRepositoryTests : RepositoryTestBase<Merchant>
    {
        private MerchantRepository _repository = null!;

        public BaseRepositoryTests()
        {
            Setup();
            _repository = new MerchantRepository(Context);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnEntity()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            await AddEntityAsync(merchant);

            // Act
            var result = await _repository.GetByIdAsync(merchant.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(merchant.Id);
            result.Name.Should().Be(merchant.Name);
            result.Email.Should().Be(merchant.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = RandomGuid();

            // Act
            var result = await _repository.GetByIdAsync(invalidId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WithEntities_ShouldReturnAllEntities()
        {
            // Arrange
            var merchants = new[]
            {
                MerchantBuilder.Create().Build(),
                MerchantBuilder.Create().Build(),
                MerchantBuilder.Create().Build()
            };
            await AddEntitiesAsync(merchants);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(m => m.Id == merchants[0].Id);
            result.Should().Contain(m => m.Id == merchants[1].Id);
            result.Should().Contain(m => m.Id == merchants[2].Id);
        }

        [Fact]
        public async Task GetAllAsync_WithNoEntities_ShouldReturnEmptyCollection()
        {
            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPagedAsync_WithValidParameters_ShouldReturnPagedResults()
        {
            // Arrange
            var merchants = new List<Merchant>();
            for (int i = 0; i < 10; i++)
            {
                merchants.Add(MerchantBuilder.Create().Build());
            }
            await AddEntitiesAsync(merchants);

            // Act
            var result = await _repository.GetPagedAsync(2, 3);

            // Assert
            result.Items.Should().HaveCount(3);
            result.TotalCount.Should().Be(10);
        }

        [Fact]
        public async Task GetPagedAsync_WithPageOutOfRange_ShouldReturnEmptyItems()
        {
            // Arrange
            var merchants = new[]
            {
                MerchantBuilder.Create().Build(),
                MerchantBuilder.Create().Build()
            };
            await AddEntitiesAsync(merchants);

            // Act
            var result = await _repository.GetPagedAsync(5, 10);

            // Assert
            result.Items.Should().BeEmpty();
            result.TotalCount.Should().Be(2);
        }

        [Fact]
        public async Task AddAsync_WithValidEntity_ShouldAddEntity()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();

            // Act
            var result = await _repository.AddAsync(merchant);
            await _repository.SaveChangesAsync();

            // Assert
            result.Should().Be(merchant);
            var savedEntity = await GetEntityByIdAsync(merchant.Id);
            savedEntity.Should().NotBeNull();
            savedEntity!.Id.Should().Be(merchant.Id);
        }

        [Fact]
        public async Task AddRangeAsync_WithValidEntities_ShouldAddAllEntities()
        {
            // Arrange
            var merchants = new[]
            {
                MerchantBuilder.Create().Build(),
                MerchantBuilder.Create().Build(),
                MerchantBuilder.Create().Build()
            };

            // Act
            var result = await _repository.AddRangeAsync(merchants);
            await _repository.SaveChangesAsync();

            // Assert
            result.Should().HaveCount(3);
            var count = await GetEntityCountAsync();
            count.Should().Be(3);
        }

        [Fact]
        public async Task UpdateAsync_WithValidEntity_ShouldUpdateEntity()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            await AddEntityAsync(merchant);
            
            var updatedName = "Updated Name";
            merchant.UpdateInformation(updatedName, merchant.Email);

            // Act
            var result = await _repository.UpdateAsync(merchant);
            await _repository.SaveChangesAsync();

            // Assert
            result.Should().Be(merchant);
            var savedEntity = await GetEntityByIdAsync(merchant.Id);
            savedEntity.Should().NotBeNull();
            savedEntity!.Name.Should().Be(updatedName);
        }

        [Fact]
        public async Task UpdateRangeAsync_WithValidEntities_ShouldUpdateAllEntities()
        {
            // Arrange
            var merchants = new[]
            {
                MerchantBuilder.Create().Build(),
                MerchantBuilder.Create().Build()
            };
            await AddEntitiesAsync(merchants);

            foreach (var merchant in merchants)
            {
                merchant.UpdateInformation("Updated Name", merchant.Email);
            }

            // Act
            var result = await _repository.UpdateRangeAsync(merchants);
            await _repository.SaveChangesAsync();

            // Assert
            result.Should().HaveCount(2);
            var allEntities = await GetAllEntitiesAsync();
            allEntities.Should().OnlyContain(m => m.Name == "Updated Name");
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteEntity()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            await AddEntityAsync(merchant);

            // Act
            var result = await _repository.DeleteAsync(merchant.Id);
            await _repository.SaveChangesAsync();

            // Assert
            result.Should().BeTrue();
            var deletedEntity = await GetEntityByIdAsync(merchant.Id);
            deletedEntity.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = RandomGuid();

            // Act
            var result = await _repository.DeleteAsync(invalidId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_WithValidEntity_ShouldDeleteEntity()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            await AddEntityAsync(merchant);

            // Act
            await _repository.DeleteAsync(merchant);
            await _repository.SaveChangesAsync();

            // Assert
            var deletedEntity = await GetEntityByIdAsync(merchant.Id);
            deletedEntity.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            await AddEntityAsync(merchant);

            // Act
            var result = await _repository.ExistsAsync(merchant.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = RandomGuid();

            // Act
            var result = await _repository.ExistsAsync(invalidId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CountAsync_WithEntities_ShouldReturnCorrectCount()
        {
            // Arrange
            var merchants = new[]
            {
                MerchantBuilder.Create().Build(),
                MerchantBuilder.Create().Build(),
                MerchantBuilder.Create().Build()
            };
            await AddEntitiesAsync(merchants);

            // Act
            var result = await _repository.CountAsync();

            // Assert
            result.Should().Be(3);
        }

        [Fact]
        public async Task CountAsync_WithNoEntities_ShouldReturnZero()
        {
            // Act
            var result = await _repository.CountAsync();

            // Assert
            result.Should().Be(0);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldSaveChangesToDatabase()
        {
            // Arrange
            var merchant = MerchantBuilder.Create().Build();
            await _repository.AddAsync(merchant);

            // Act
            var result = await _repository.SaveChangesAsync();

            // Assert
            result.Should().BeGreaterThan(0);
            var savedEntity = await GetEntityByIdAsync(merchant.Id);
            savedEntity.Should().NotBeNull();
        }

        public void Dispose()
        {
            Cleanup();
        }
    }
}
