using BalanceApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BalanceApi.Api.Controllers;

[ApiController]
public class BalanceController : ControllerBase
{
    private readonly AccountService _accountService;

    public BalanceController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("balance")]
    public IActionResult GetBalance([FromQuery] string account_id)
    {
        var balance = _accountService.GetBalance(account_id);

        if (balance == null)
        {
            return NotFound(0);
        }

        return Ok(balance.Value);
    }
}