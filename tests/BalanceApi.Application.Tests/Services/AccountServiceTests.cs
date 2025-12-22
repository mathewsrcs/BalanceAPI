using BalanceApi.Application.DTOs;
using BalanceApi.Application.Services;
using BalanceApi.Domain.Entities;
using BalanceApi.Domain.Interfaces;

namespace BalanceApi.Application.Tests.Services;

public class AccountServiceTests
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
    public void GetBalance_ShouldReturnNullWhenAccountDoesNotExist(string accountId)
    {
        // Arrange
        var repository = new MockAccountRepository();
        var service = new AccountService(repository);

        // Act
        var balance = service.GetBalance(accountId);

        // Assert
        Assert.Null(balance);
    }

    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100.50)]
    [InlineData("300", 0)]
    public void GetBalance_ShouldReturnBalanceWhenAccountExists(string accountId, decimal expectedBalance)
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account(accountId, expectedBalance));
        var service = new AccountService(repository);

        // Act
        var balance = service.GetBalance(accountId);

        // Assert
        Assert.Equal(expectedBalance, balance);
    }

    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100.50)]
    [InlineData("300", 25)]
    public void ProcessEvent_Deposit_ShouldCreateNewAccountWhenNotExists(string accountId, decimal amount)
    {
        // Arrange
        var repository = new MockAccountRepository();
        var service = new AccountService(repository);
        var request = new EventRequest
        {
            Type = "deposit",
            Destination = accountId,
            Amount = amount
        };

        // Act
        var response = service.ProcessEvent(request);

        // Assert
        Assert.NotNull(response.Destination);
        Assert.Equal(accountId, response.Destination.Id);
        Assert.Equal(amount, response.Destination.Balance);
        Assert.Null(response.Origin);
    }

    [Theory]
    [InlineData("100", 50, 25, 75)]
    [InlineData("200", 100, 50.50, 150.50)]
    [InlineData("300", 0, 100, 100)]
    public void ProcessEvent_Deposit_ShouldAddToExistingAccount(string accountId, decimal initialBalance, decimal depositAmount, decimal expectedBalance)
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account(accountId, initialBalance));
        var service = new AccountService(repository);
        var request = new EventRequest
        {
            Type = "deposit",
            Destination = accountId,
            Amount = depositAmount
        };

        // Act
        var response = service.ProcessEvent(request);

        // Assert
        Assert.NotNull(response.Destination);
        Assert.Equal(accountId, response.Destination.Id);
        Assert.Equal(expectedBalance, response.Destination.Balance);
        Assert.Null(response.Origin);
    }

    [Theory]
    [InlineData("100", 50, 25, 25)]
    [InlineData("200", 100, 50, 50)]
    [InlineData("300", 100, 100, 0)]
    public void ProcessEvent_Withdraw_ShouldDeductFromExistingAccount(string accountId, decimal initialBalance, decimal withdrawAmount, decimal expectedBalance)
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account(accountId, initialBalance));
        var service = new AccountService(repository);
        var request = new EventRequest
        {
            Type = "withdraw",
            Origin = accountId,
            Amount = withdrawAmount
        };

        // Act
        var response = service.ProcessEvent(request);

        // Assert
        Assert.NotNull(response.Origin);
        Assert.Equal(accountId, response.Origin.Id);
        Assert.Equal(expectedBalance, response.Origin.Balance);
        Assert.Null(response.Destination);
    }

    [Theory]
    [InlineData("999")]
    [InlineData("888")]
    [InlineData("777")]
    public void ProcessEvent_Withdraw_ShouldThrowWhenAccountDoesNotExist(string accountId)
    {
        // Arrange
        var repository = new MockAccountRepository();
        var service = new AccountService(repository);
        var request = new EventRequest
        {
            Type = "withdraw",
            Origin = accountId,
            Amount = 50
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.ProcessEvent(request));
    }

    [Theory]
    [InlineData("100", 100, "200", 50, 50, 50)]
    [InlineData("300", 150, "400", 75, 75, 75)]
    [InlineData("500", 200, "600", 100, 100, 100)]
    public void ProcessEvent_Transfer_ShouldTransferBetweenExistingAccounts(string originId, decimal originInitial, string destId, decimal destInitial, decimal transferAmount, decimal expectedBalance)
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account(originId, originInitial));
        repository.CreateAccount(new Account(destId, destInitial));
        var service = new AccountService(repository);
        var request = new EventRequest
        {
            Type = "transfer",
            Origin = originId,
            Destination = destId,
            Amount = transferAmount
        };

        // Act
        var response = service.ProcessEvent(request);

        // Assert
        Assert.NotNull(response.Origin);
        Assert.Equal(originId, response.Origin.Id);
        Assert.Equal(expectedBalance, response.Origin.Balance);
        Assert.NotNull(response.Destination);
        Assert.Equal(destId, response.Destination.Id);
        Assert.Equal(expectedBalance + transferAmount, response.Destination.Balance);
    }

    [Theory]
    [InlineData("100", 100, "200", 50)]
    [InlineData("300", 150, "400", 75)]
    [InlineData("500", 200, "600", 100)]
    public void ProcessEvent_Transfer_ShouldCreateDestinationAccountWhenNotExists(string originId, decimal originInitial, string destId, decimal transferAmount)
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account(originId, originInitial));
        var service = new AccountService(repository);
        var request = new EventRequest
        {
            Type = "transfer",
            Origin = originId,
            Destination = destId,
            Amount = transferAmount
        };

        // Act
        var response = service.ProcessEvent(request);

        // Assert
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
    public void ProcessEvent_Transfer_ShouldThrowWhenOriginAccountDoesNotExist(string originId)
    {
        // Arrange
        var repository = new MockAccountRepository();
        var service = new AccountService(repository);
        var request = new EventRequest
        {
            Type = "transfer",
            Origin = originId,
            Destination = "200",
            Amount = 50
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => service.ProcessEvent(request));
    }

    [Fact]
    public void Reset_ShouldClearAllAccounts()
    {
        // Arrange
        var repository = new MockAccountRepository();
        repository.CreateAccount(new Account("100", 50));
        repository.CreateAccount(new Account("200", 100));
        var service = new AccountService(repository);

        // Act
        service.Reset();

        // Assert
        Assert.Null(service.GetBalance("100"));
        Assert.Null(service.GetBalance("200"));
    }
}