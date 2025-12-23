using BalanceApi.Api.Controllers;
using BalanceApi.Application.Services;
using BalanceApi.Domain.Entities;
using BalanceApi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalanceApi.Api.Tests.Controllers;

public class BalanceControllerTests
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
    [InlineData("100")]
    [InlineData("200")]
    [InlineData("300")]
    public void GetBalance_ShouldReturnNotFoundWhenAccountDoesNotExist(string accountId)
    {
        // Arrange
        var repository = new MockAccountRepository();
        var service = new AccountService(repository);
        var controller = new BalanceController(service);

        // Act
        var result = controller.GetBalance(accountId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(0, notFoundResult.Value);
    }

    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100.50)]
    [InlineData("300", 0)]
    public void GetBalance_ShouldReturnOkWithBalanceWhenAccountExists(string accountId, decimal balance)
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account(accountId, balance));
        var service = new AccountService(repository);
        var controller = new BalanceController(service);

        // Act
        var result = controller.GetBalance(accountId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(balance, okResult.Value);
    }
}