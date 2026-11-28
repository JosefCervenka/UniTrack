using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

public class StatusController : ControllerBase
{
    private readonly StatusService _statusService;
    
    public StatusController(StatusService statusService)
    {
        _statusService = statusService;
    }
    
    [HttpGet("/api/status")]
    public async Task<IActionResult> GetAsync()
    {
        try
        {
            var result = await _statusService.GetAllAsync();
            return Ok(result);
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred");
        }
    }
}