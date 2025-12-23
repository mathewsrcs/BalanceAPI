using BalanceApi.Application.DTOs;
using BalanceApi.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BalanceApi.Api.Controllers;

[ApiController]
public class EventController : ControllerBase
{
    private readonly AccountService _accountService;

    public EventController(AccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("event")]
    public IActionResult ProcessEvent([FromBody] EventRequest request)
    {
        try
        {
            var response = _accountService.ProcessEvent(request);
            return Created(string.Empty, response);
        }
        catch (InvalidOperationException)
        {
            return NotFound(0);
        }
    }
}