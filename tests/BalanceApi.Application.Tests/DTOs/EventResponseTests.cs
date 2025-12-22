using BalanceApi.Application.DTOs;

namespace BalanceApi.Application.Tests.DTOs;

public class EventResponseTests
{
    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100.50)]
    [InlineData("300", 0)]
    public void AccountDto_ShouldSetPropertiesCorrectly(string id, decimal balance)
    {
        // Act
        var accountDto = new AccountDto
        {
            Id = id,
            Balance = balance
        };

        // Assert
        Assert.Equal(id, accountDto.Id);
        Assert.Equal(balance, accountDto.Balance);
    }

    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100.50)]
    [InlineData("300", 0)]
    public void EventResponse_ShouldSetOriginOnly(string id, decimal balance)
    {
        // Arrange
        var origin = new AccountDto { Id = id, Balance = balance };

        // Act
        var response = new EventResponse
        {
            Origin = origin,
            Destination = null
        };

        // Assert
        Assert.NotNull(response.Origin);
        Assert.Equal(id, response.Origin.Id);
        Assert.Equal(balance, response.Origin.Balance);
        Assert.Null(response.Destination);
    }

    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100.50)]
    [InlineData("300", 0)]
    public void EventResponse_ShouldSetDestinationOnly(string id, decimal balance)
    {
        // Arrange
        var destination = new AccountDto { Id = id, Balance = balance };

        // Act
        var response = new EventResponse
        {
            Origin = null,
            Destination = destination
        };

        // Assert
        Assert.Null(response.Origin);
        Assert.NotNull(response.Destination);
        Assert.Equal(id, response.Destination.Id);
        Assert.Equal(balance, response.Destination.Balance);
    }

    [Theory]
    [InlineData("100", 25, "200", 75)]
    [InlineData("300", 50, "400", 150)]
    [InlineData("500", 0, "600", 100)]
    public void EventResponse_ShouldSetBothOriginAndDestination(string originId, decimal originBalance, string destId, decimal destBalance)
    {
        // Arrange
        var origin = new AccountDto { Id = originId, Balance = originBalance };
        var destination = new AccountDto { Id = destId, Balance = destBalance };

        // Act
        var response = new EventResponse
        {
            Origin = origin,
            Destination = destination
        };

        // Assert
        Assert.NotNull(response.Origin);
        Assert.Equal(originId, response.Origin.Id);
        Assert.Equal(originBalance, response.Origin.Balance);
        Assert.NotNull(response.Destination);
        Assert.Equal(destId, response.Destination.Id);
        Assert.Equal(destBalance, response.Destination.Balance);
    }
}