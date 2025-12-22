using BalanceApi.Application.DTOs;
using BalanceApi.Domain.Entities;
using BalanceApi.Domain.Interfaces;

namespace BalanceApi.Application.Services;

public class AccountService
{
    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public decimal? GetBalance(string accountId)
    {
        var account = _accountRepository.GetAccount(accountId);
        return account?.Balance;
    }

    public EventResponse ProcessEvent(EventRequest request)
    {
        return request.Type.ToLower() switch
        {
            "deposit" => ProcessDeposit(request.Destination!, request.Amount),
            "withdraw" => ProcessWithdraw(request.Origin!, request.Amount),
            "transfer" => ProcessTransfer(request.Origin!, request.Destination!, request.Amount),
            _ => throw new ArgumentException("Invalid event type")
        };
    }

    public void Reset()
    {
        _accountRepository.Reset();
    }

    private EventResponse ProcessDeposit(string destinationId, decimal amount)
    {
        var account = _accountRepository.GetAccount(destinationId);

        if (account == null)
        {
            account = new Account(destinationId, amount);
            _accountRepository.CreateAccount(account);
        }
        else
        {
            account.Balance += amount;
            _accountRepository.UpdateAccount(account);
        }

        return new EventResponse
        {
            Destination = new AccountDto { Id = account.Id, Balance = account.Balance }
        };
    }

    private EventResponse ProcessWithdraw(string originId, decimal amount)
    {
        var account = _accountRepository.GetAccount(originId);

        if (account == null)
        {
            throw new InvalidOperationException("Account not found");
        }

        account.Balance -= amount;
        _accountRepository.UpdateAccount(account);

        return new EventResponse
        {
            Origin = new AccountDto { Id = account.Id, Balance = account.Balance }
        };
    }

    private EventResponse ProcessTransfer(string originId, string destinationId, decimal amount)
    {
        var originAccount = _accountRepository.GetAccount(originId);

        if (originAccount == null)
        {
            throw new InvalidOperationException("Origin account not found");
        }

        var destinationAccount = _accountRepository.GetAccount(destinationId);

        if (destinationAccount == null)
        {
            destinationAccount = new Account(destinationId, amount);
            _accountRepository.CreateAccount(destinationAccount);
        }
        else
        {
            destinationAccount.Balance += amount;
            _accountRepository.UpdateAccount(destinationAccount);
        }

        originAccount.Balance -= amount;
        _accountRepository.UpdateAccount(originAccount);

        return new EventResponse
        {
            Origin = new AccountDto { Id = originAccount.Id, Balance = originAccount.Balance },
            Destination = new AccountDto { Id = destinationAccount.Id, Balance = destinationAccount.Balance }
        };
    }
}