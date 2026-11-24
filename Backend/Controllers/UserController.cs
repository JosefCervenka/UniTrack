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
            return BadRequest("An unexpected error occurred");
        }

        return Created();
    }

    [HttpGet("/api/user/login")]
    public async Task<IActionResult> Login(LoginUser loginUser)
    {
        try
        {
            await _userService.Login(loginUser);
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Errors.Select(error => error.ErrorMessage));
        }
        catch
        {
            return BadRequest("An unexpected error occurred");
        }

        return Ok();
    }
}