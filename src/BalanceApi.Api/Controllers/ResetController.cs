using BalanceApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BalanceApi.Api.Controllers;

[ApiController]
public class ResetController : ControllerBase
{
    private readonly AccountService _accountService;

    public ResetController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("reset")]
    public IActionResult Reset()
    {
        _accountService.Reset();
        return Ok("OK");
    }
}