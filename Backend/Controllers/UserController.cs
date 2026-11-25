using Application.Dtos;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[AllowAnonymous]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("/api/user/logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            HttpContext.Response.Cookies.Delete("jwt");
            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }

    [HttpPost("/api/user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUser user)
    {
        try
        {
            await _userService.CreateUser(user);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Errors.Select(error => error.ErrorMessage));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }

        return Created();
    }

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
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Errors.Select(error => error.ErrorMessage));
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }

        return Ok();
    }
}