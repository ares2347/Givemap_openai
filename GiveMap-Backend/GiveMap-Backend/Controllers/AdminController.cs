using System.Security.Claims;
using GiveMap_Backend.Models;
using GiveMap_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiveMap_Backend.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILocationService _locationService;
    private readonly IReportService _reportService;
    private readonly ILogger<AdminController> _logger;

    public AdminController(UserManager<User> userManager, ILocationService locationService,
        IReportService reportService,
        ILogger<AdminController> logger)
    {
        _userManager = userManager;
        _locationService = locationService;
        _reportService = reportService;
        _logger = logger;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(users);
    }

    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserModel model)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Email = model.Email;
        user.UserName = model.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return Ok(user);
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            return NoContent();
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("locations")]
    public async Task<IActionResult> AddLocation([FromBody] AddLocationModel model)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var location = await _locationService.AddLocationAsync(model.Latitude, model.Longitude, model.Name, model.Description, model.Category, userId);
            return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding location");
            return StatusCode(500, new { message = "An error occurred while adding the location" });
        }
    }

    [HttpGet("locations/{id}")]
    public async Task<IActionResult> GetLocation(int id)
    {
        try
        {
            var location = await _locationService.GetLocationDetailsAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving location with ID: {id}");
            return StatusCode(500, new { message = "An error occurred while retrieving the location" });
        }
    }

    [HttpPut("locations/{id}")]
    public async Task<IActionResult> UpdateLocation(int id, [FromBody] UpdateLocationModel model)
    {
        try
        {
            var location =
                await _locationService.UpdateLocationDetailsAsync(id, model.Description, model.Category,
                    model.ImageUrls);
            if (location == null)
            {
                return NotFound();
            }

            return Ok(location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating location with ID: {id}");
            return StatusCode(500, new { message = "An error occurred while updating the location" });
        }
    }

    [HttpGet("locations")]
    public async Task<IActionResult> GetLocations([FromQuery] GetLocationsModel model)
    {
        try
        {
            var locations = await _locationService.GetLocationsAsync(model.Keyword, model.Category, model.FromDate);
            return Ok(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving locations");
            return StatusCode(500, new { message = "An error occurred while retrieving locations" });
        }
    }

    [HttpDelete("locations/{id}")]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        try
        {
            var location = await _locationService.GetLocationDetailsAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            await _locationService.DeleteLocationAsync(id);
            _logger.LogInformation($"Location with ID {id} has been deleted.");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting location with ID: {id}");
            return StatusCode(500, new { message = "An error occurred while deleting the location" });
        }
    }

    [HttpGet("reports/user-activity")]
    public async Task<IActionResult> GenerateUserActivityReport([FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var report = await _reportService.GenerateUserActivityReportAsync(startDate, endDate);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating user activity report");
            return StatusCode(500, new { message = "An error occurred while generating the user activity report" });
        }
    }

    [HttpGet("reports/location-data")]
    public async Task<IActionResult> GenerateLocationDataReport([FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var report = await _reportService.GenerateLocationDataReportAsync(startDate, endDate);
            return Ok(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating location data report");
            return StatusCode(500, new { message = "An error occurred while generating the location data report" });
        }
    }
}

public class UpdateUserModel
{
    public string Email { get; set; }
    public string UserName { get; set; }
}

public class UpdateLocationModel
{
    public string Description { get; set; }
    public string Category { get; set; }
    public List<string> ImageUrls { get; set; }
}

public class GetLocationsModel
{
    public string Keyword { get; set; }
    public string Category { get; set; }
    public string Urgency { get; set; }
    public DateTime? FromDate { get; set; }
}