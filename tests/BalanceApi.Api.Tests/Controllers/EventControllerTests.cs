using BalanceApi.Api.Controllers;
using BalanceApi.Application.DTOs;
using BalanceApi.Application.Services;
using BalanceApi.Domain.Entities;
using BalanceApi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalanceApi.Api.Tests.Controllers;

public class EventControllerTests
{
    private class MockAccountRepository : IAccountRepository
    {
        private readonly Dictionary<string, Account> _accounts = new();

        public Account? GetAccount(string id)
        {
            return _accounts.TryGetValue(id, out var account) ? account : null;
        }

        public void CreateAccount(Account account)
        {
            _accounts[account.Id] = account;
        }

        public void UpdateAccount(Account account)
        {
            _accounts[account.Id] = account;
        }

        public bool AccountExists(string id)
        {
            return _accounts.ContainsKey(id);
        }

        public void Reset()
        {
            _accounts.Clear();
        }
    }

    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100.50)]
    [InlineData("300", 25)]
    public void ProcessEvent_Deposit_ShouldReturnCreatedWithDestination(string accountId, decimal amount)
    {
        // Arrange
        var repository = new MockAccountRepository();
        var service = new AccountService(repository);
        var controller = new EventController(service);
        var request = new EventRequest
        {
            Type = "deposit",
            Destination = accountId,
            Amount = amount
        };

        // Act
        var result = controller.ProcessEvent(request);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        var response = Assert.IsType<EventResponse>(createdResult.Value);
        Assert.NotNull(response.Destination);
        Assert.Equal(accountId, response.Destination.Id);
        Assert.Equal(amount, response.Destination.Balance);
        Assert.Null(response.Origin);
    }

    [Theory]
    [InlineData("100", 100, 50, 50)]
    [InlineData("200", 150, 75, 75)]
    [InlineData("300", 200, 100, 100)]
    public void ProcessEvent_Withdraw_ShouldReturnCreatedWithOrigin(string accountId, decimal initialBalance, decimal withdrawAmount, decimal expectedBalance)
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account(accountId, initialBalance));
        var service = new AccountService(repository);
        var controller = new EventController(service);
        var request = new EventRequest
        {
            Type = "withdraw",
            Origin = accountId,
            Amount = withdrawAmount
        };

        // Act
        var result = controller.ProcessEvent(request);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        var response = Assert.IsType<EventResponse>(createdResult.Value);
        Assert.NotNull(response.Origin);
        Assert.Equal(accountId, response.Origin.Id);
        Assert.Equal(expectedBalance, response.Origin.Balance);
        Assert.Null(response.Destination);
    }

    [Theory]
    [InlineData("999")]
    [InlineData("888")]
    [InlineData("777")]
    public void ProcessEvent_Withdraw_ShouldReturnNotFoundWhenAccountDoesNotExist(string accountId)
    {
        // Arrange
        var repository = new MockAccountRepository();
        var service = new AccountService(repository);
        var controller = new EventController(service);
        var request = new EventRequest
        {
            Type = "withdraw",
            Origin = accountId,
            Amount = 50
        };

        // Act
        var result = controller.ProcessEvent(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(0, notFoundResult.Value);
    }

    [Theory]
    [InlineData("100", 100, "200", 50)]
    [InlineData("300", 150, "400", 75)]
    [InlineData("500", 200, "600", 100)]
    public void ProcessEvent_Transfer_ShouldReturnCreatedWithBothAccounts(string originId, decimal originInitial, string destId, decimal transferAmount)
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account(originId, originInitial));
        var service = new AccountService(repository);
        var controller = new EventController(service);
        var request = new EventRequest
        {
            Type = "transfer",
            Origin = originId,
            Destination = destId,
            Amount = transferAmount
        };

        // Act
        var result = controller.ProcessEvent(request);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        var response = Assert.IsType<EventResponse>(createdResult.Value);
        Assert.NotNull(response.Origin);
        Assert.Equal(originId, response.Origin.Id);
        Assert.Equal(originInitial - transferAmount, response.Origin.Balance);
        Assert.NotNull(response.Destination);
        Assert.Equal(destId, response.Destination.Id);
        Assert.Equal(transferAmount, response.Destination.Balance);
    }

    [Theory]
    [InlineData("999")]
    [InlineData("888")]
    [InlineData("777")]
    public void ProcessEvent_Transfer_ShouldReturnNotFoundWhenOriginDoesNotExist(string originId)
    {
        // Arrange
        var repository = new MockAccountRepository();
        var service = new AccountService(repository);
        var controller = new EventController(service);
        var request = new EventRequest
        {
            Type = "transfer",
            Origin = originId,
            Destination = "200",
            Amount = 50
        };

        // Act
        var result = controller.ProcessEvent(request);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(0, notFoundResult.Value);
    }
}