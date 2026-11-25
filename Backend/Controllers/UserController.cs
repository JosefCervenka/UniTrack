using Application.Dtos;
using Application.Services;
using Backend.Adapters;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("/api/user/current")]
    public async Task<IActionResult> GetCurrent()
    {
        try
        {
            return Ok(await _userService.GetCurrentUser());
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("User is not authorized.");
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    [Authorize]
    [HttpPost("/api/user/logout")]
    public IActionResult Logout()
    {
        try
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return Ok();
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    [AllowAnonymous]
    [HttpPost("/api/user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUser user)
    {
        try
        {
            await _userService.CreateUser(user);
            return Created();
        }
        catch (ValidationException e)
        {
            return ErrorAdapter.ToActionResult(e);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    [AllowAnonymous]
    [HttpPost("/api/user/login")]
    public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
    {
        try
        {
            var token = await _userService.Login(loginUser);
            HttpContext.Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true
            });
            return Ok();
        }
        catch (ValidationException e)
        {
            return ErrorAdapter.ToActionResult(e);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }
}