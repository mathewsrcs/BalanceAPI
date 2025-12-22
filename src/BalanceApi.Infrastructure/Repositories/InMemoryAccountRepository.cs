using BalanceApi.Domain.Entities;
using BalanceApi.Domain.Interfaces;

namespace BalanceApi.Infrastructure.Repositories;

public class InMemoryAccountRepository : IAccountRepository
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