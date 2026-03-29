using BookstoreWeb.Application.DTOs.Account;
using BookstoreWeb.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreWeb.API.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    //tạo tkhoan mới
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        await _accountService.RegisterAsync(request);
        return NoContent();
    }

    //login, trả về jwt token
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _accountService.LoginAsync(request);
        return Ok(response);
    }
}
