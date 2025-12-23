using BalanceApi.Api.Controllers;
using BalanceApi.Application.Services;
using BalanceApi.Domain.Entities;
using BalanceApi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BalanceApi.Api.Tests.Controllers;

public class ResetControllerTests
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

    [Fact]
    public void Reset_ShouldReturnOkWithMessage()
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account("100", 50));
        repository.CreateAccount(new Account("200", 100));
        var service = new AccountService(repository);
        var controller = new ResetController(service);

        // Act
        var result = controller.Reset();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("OK", okResult.Value);
    }

    [Fact]
    public void Reset_ShouldClearAllAccounts()
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account("100", 50));
        repository.CreateAccount(new Account("200", 100));
        var service = new AccountService(repository);
        var controller = new ResetController(service);

        // Act
        controller.Reset();

        // Assert
        Assert.Null(service.GetBalance("100"));
        Assert.Null(service.GetBalance("200"));
    }
}