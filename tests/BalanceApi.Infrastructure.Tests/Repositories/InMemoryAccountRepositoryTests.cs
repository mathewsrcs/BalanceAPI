using BalanceApi.Domain.Entities;
using BalanceApi.Infrastructure.Repositories;

namespace BalanceApi.Infrastructure.Tests.Repositories;

public class InMemoryAccountRepositoryTests
{
    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100)]
    [InlineData("300", 0)]
    public void CreateAccount_ShouldAddAccountToRepository(string id, decimal balance)
    {
        // Arrange
        var repository = new InMemoryAccountRepository();
        var account = new Account(id, balance);

        // Act
        repository.CreateAccount(account);

        // Assert
        Assert.True(repository.AccountExists(id));
    }

    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 100.50)]
    [InlineData("300", 0)]
    public void GetAccount_ShouldReturnAccountWhenExists(string id, decimal balance)
    {
        // Arrange
        var repository = new InMemoryAccountRepository();
        var account = new Account(id, balance);
        repository.CreateAccount(account);

        // Act
        var result = repository.GetAccount(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(balance, result.Balance);
    }

    [Theory]
    [InlineData("999")]
    [InlineData("888")]
    [InlineData("777")]
    public void GetAccount_ShouldReturnNullWhenNotExists(string id)
    {
        // Arrange
        var repository = new InMemoryAccountRepository();

        // Act
        var result = repository.GetAccount(id);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("100", 50, 150)]
    [InlineData("200", 100, 200)]
    [InlineData("300", 0, 75.50)]
    public void UpdateAccount_ShouldModifyExistingAccount(string id, decimal initialBalance, decimal newBalance)
    {
        // Arrange
        var repository = new InMemoryAccountRepository();
        var account = new Account(id, initialBalance);
        repository.CreateAccount(account);

        // Act
        account.Balance = newBalance;
        repository.UpdateAccount(account);
        var result = repository.GetAccount(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newBalance, result.Balance);
    }

    [Theory]
    [InlineData("100", true)]
    [InlineData("200", true)]
    [InlineData("999", false)]
    public void AccountExists_ShouldReturnCorrectValue(string id, bool shouldExist)
    {
        // Arrange
        var repository = new InMemoryAccountRepository();
        if (shouldExist)
        {
            repository.CreateAccount(new Account(id, 50));
        }

        // Act
        var exists = repository.AccountExists(id);

        // Assert
        Assert.Equal(shouldExist, exists);
    }

    [Fact]
    public void Reset_ShouldRemoveAllAccounts()
    {
        // Arrange
        var repository = new InMemoryAccountRepository();
        repository.CreateAccount(new Account("100", 50));
        repository.CreateAccount(new Account("200", 100));

        // Act
        repository.Reset();

        // Assert
        Assert.False(repository.AccountExists("100"));
        Assert.False(repository.AccountExists("200"));
    }
}