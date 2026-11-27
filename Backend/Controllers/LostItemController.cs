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

    [HttpGet("/api/lost-items")]
    public async Task<IActionResult> GetAsync([FromQuery] int skip, [FromQuery] int take, [FromQuery] string search)
    {
        try
        {
            var result = await _lostItemService.GetAllAsync(search, take, skip);
            return Ok(result);
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

    [HttpDelete("/api/lost-items/{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
    {
        try
        {
            await _lostItemService.DeleteAsync(id);
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