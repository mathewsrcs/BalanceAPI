namespace BalanceApi.Application.DTOs;

public class EventRequest
{
    public string Type { get; set; } = string.Empty;
    public string? Destination { get; set; }
    public string? Origin { get; set; }
    public decimal Amount { get; set; }
}