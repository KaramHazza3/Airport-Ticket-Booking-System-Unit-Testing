using AirportTicketBookingSystem.Repositories;

namespace AirportTicketBookingSystem.Tests;

public class RepositoryTests
{
    [Fact]
    public void GetInstance_ShouldReturnRepositoryInstance()
    {
        // Arrange & Act
        var result = FileRepository.Instance;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<FileRepository>(result);
    }

    [Fact]
    public void GetInstance_ShouldReturnSameRepositoryInstance()
    {
        // Arrange
        var repo1 = FileRepository.Instance;
        var repo2 = FileRepository.Instance;

        // Act
        var result = repo1 == repo2;

        // Assert
        Assert.True(result);
    }
}