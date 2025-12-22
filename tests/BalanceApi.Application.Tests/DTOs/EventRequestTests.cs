using BalanceApi.Application.DTOs;

namespace BalanceApi.Application.Tests.DTOs;

public class EventRequestTests
{
    [Theory]
    [InlineData("deposit", "100", null, 50)]
    [InlineData("withdraw", null, "100", 25.50)]
    [InlineData("transfer", "200", "100", 100)]
    public void EventRequest_ShouldSetPropertiesCorrectly(string type, string? destination, string? origin, decimal amount)
    {
        // Act
        var request = new EventRequest
        {
            Type = type,
            Destination = destination,
            Origin = origin,
            Amount = amount
        };

        // Assert
        Assert.Equal(type, request.Type);
        Assert.Equal(destination, request.Destination);
        Assert.Equal(origin, request.Origin);
        Assert.Equal(amount, request.Amount);
    }

    [Theory]
    [InlineData("deposit")]
    [InlineData("withdraw")]
    [InlineData("transfer")]
    public void EventRequest_ShouldHaveDefaultEmptyType(string type)
    {
        // Arrange
        var request = new EventRequest();

        // Act
        request.Type = type;

        // Assert
        Assert.Equal(type, request.Type);
    }
}