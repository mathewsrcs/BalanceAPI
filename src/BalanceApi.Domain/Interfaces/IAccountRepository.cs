using BalanceApi.Domain.Entities;

namespace BalanceApi.Domain.Interfaces;

public interface IAccountRepository
{
    Account? GetAccount(string id);
    void CreateAccount(Account account);
    void UpdateAccount(Account account);
    bool AccountExists(string id);
    void Reset();
}