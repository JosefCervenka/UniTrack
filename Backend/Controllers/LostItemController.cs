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

    [HttpGet("/api/lost-items/{id:guid}/picture")]
    public async Task<IActionResult> GetPictureAsync([FromRoute] Guid id)
    {
        try
        {
            var image = await _lostItemService.GetPictureAsync(id);
            if (image is null || image.Length == 0)
                return NotFound();

            return File(image, GetPictureContentType(image));
        }
        catch (ValidationException e)
        {
            return ErrorAdapter.ToActionResult(e);
        }
    }

    private static string GetPictureContentType(byte[] image)
    {
        if (image.AsSpan().StartsWith(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
            return "image/png";
        if (image.AsSpan().StartsWith(new byte[] { 0xFF, 0xD8, 0xFF }))
            return "image/jpeg";
        if (image.AsSpan().StartsWith("GIF"u8))
            return "image/gif";
        if (image.Length >= 12 &&
            image.AsSpan(0, 4).SequenceEqual("RIFF"u8) &&
            image.AsSpan(8, 4).SequenceEqual("WEBP"u8))
            return "image/webp";
        if (image.AsSpan().StartsWith("BM"u8))
            return "image/bmp";

        return "application/octet-stream";
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