namespace BalanceApi.Application.DTOs;

public class EventResponse
{
    public AccountDto? Origin { get; set; }
    public AccountDto? Destination { get; set; }
}

public class AccountDto
{
    public string Id { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}