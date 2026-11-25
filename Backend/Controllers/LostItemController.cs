using System.ComponentModel.DataAnnotations;
using System.Formats.Asn1;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class LostItemController : ControllerBase
{
    LostItemService _lostItemService;

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
            return Created();
        }
        catch (ValidationException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
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
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }
}