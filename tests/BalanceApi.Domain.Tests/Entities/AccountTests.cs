using BalanceApi.Domain.Entities;

namespace BalanceApi.Domain.Tests.Entities;

public class AccountTests
{
    [Theory]
    [InlineData("100", 50)]
    [InlineData("200", 0)]
    [InlineData("300", 100.50)]
    public void Account_ShouldCreateWithIdAndBalance(string id, decimal balance)
    {
        // Act
        var account = new Account(id, balance);

        // Assert
        Assert.Equal(id, account.Id);
        Assert.Equal(balance, account.Balance);
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(50, 150)]
    [InlineData(100, 0)]
    public void Account_ShouldAllowBalanceUpdate(decimal initialBalance, decimal newBalance)
    {
        // Arrange
        var account = new Account("100", initialBalance);

        // Act
        account.Balance = newBalance;

        // Assert
        Assert.Equal(newBalance, account.Balance);
    }
}