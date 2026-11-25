using Application.Dtos;
using Application.Services;
using Backend.Adapters;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
public class LostItemController : ControllerBase
{
    private readonly LostItemService _lostItemService;

    public LostItemController(LostItemService lostItemService)
    {
        _lostItemService = lostItemService;
    }

    [HttpPut("/api/lost-items")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateLostItem updateLostItem)
    {
        try
        {
            await _lostItemService.UpdateAsync(updateLostItem);
            return NoContent();
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

    [HttpPost("/api/lost-items")]
    public async Task<IActionResult> CreateAsync([FromForm] CreateLostItem createLostItem)
    {
        try
        {
            await _lostItemService.CreateAsync(createLostItem);
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
}