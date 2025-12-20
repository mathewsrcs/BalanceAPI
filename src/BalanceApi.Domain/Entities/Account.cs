namespace BalanceApi.Domain.Entities;

public class Account
{
    public string Id { get; set; }
    public decimal Balance { get; set; }

    public Account(string id, decimal balance)
    {
        Id = id;
        Balance = balance;
    }
}