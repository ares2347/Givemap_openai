using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using GiveMap_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GiveMap_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ILogger<LocationController> _logger;
    private readonly IEmailService _emailService;
    private readonly ILocationFeedbackService _feedbackService;
    private readonly IWebHostEnvironment _environment;

    public LocationController(ILocationService locationService, ILogger<LocationController> logger,
        IEmailService emailService,
        IWebHostEnvironment environment, ILocationFeedbackService feedbackService)
    {
        _locationService = locationService;
        _logger = logger;
        _emailService = emailService;
        _environment = environment;
        _feedbackService = feedbackService;
    }

    [HttpPost]
    public async Task<IActionResult> AddLocation([FromBody] AddLocationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var location = await _locationService.AddLocationAsync(model.Latitude, model.Longitude, model.Name, model.Description, model.Category ,userId);
            return CreatedAtAction(nameof(GetLocations), new { id = location.Id }, location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding location");
            return StatusCode(500, new { message = "An error occurred while adding the location" });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetLocations([FromQuery] LocationFilterModel filter)
    {
        try
        {
            var locations = await _locationService.GetLocationsAsync(filter.Keyword, filter.Category, filter.FromDate);
            return Ok(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving locations");
            return StatusCode(500, new { message = "An error occurred while retrieving locations" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLocationDetails(int id)
    {
        try
        {
            var location = await _locationService.GetLocationDetailsAsync(id);
            if (location == null)
            {
                return NotFound(new { message = "Location not found" });
            }

            return Ok(location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving location details for ID: {id}");
            return StatusCode(500, new { message = "An error occurred while retrieving location details" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLocationDetails(int id, [FromForm] UpdateLocationDetailsModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var imageUrls = new List<string>();
            if (model.Images != null && model.Images.Count > 0)
            {
                foreach (var image in model.Images)
                {
                    var imageUrl = await SaveImageAsync(image);
                    imageUrls.Add(imageUrl);
                }
            }

            var location =
                await _locationService.UpdateLocationDetailsAsync(id, model.Description, model.Category, imageUrls);
            return Ok(location);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating location details");
            return StatusCode(500, new { message = "An error occurred while updating location details" });
        }
    }

    [HttpPost("{id}/feedback")]
    [Authorize]
    public async Task<IActionResult> AddFeedback(int id, [FromBody] AddFeedbackModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var feedback = await _feedbackService.AddFeedbackAsync(id, userId, model.Comment, model.Rating);
            return CreatedAtAction(nameof(GetLocationFeedback), new { id = feedback.Id }, feedback);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding feedback");
            return StatusCode(500, new { message = "An error occurred while adding feedback" });
        }
    }

    [HttpGet("{id}/feedback")]
    public async Task<IActionResult> GetLocationFeedback(int id)
    {
        try
        {
            var feedback = await _feedbackService.GetLocationFeedbackAsync(id);
            var averageRating = await _feedbackService.GetAverageRatingAsync(id);
            return Ok(new { Feedback = feedback, AverageRating = averageRating });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving location feedback");
            return StatusCode(500, new { message = "An error occurred while retrieving location feedback" });
        }
    }

    private async Task<string> SaveImageAsync(IFormFile image)
    {
        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
        var filePath = Path.Combine(uploadPath, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return $"/uploads/{fileName}";
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchLocations([FromQuery] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest(new { message = "Search keyword is required" });
        }

        try
        {
            var locations = await _locationService.SearchLocationsAsync(keyword);
            return Ok(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error searching locations with keyword: {keyword}");
            return StatusCode(500, new { message = "An error occurred while searching locations" });
        }
    }

    [HttpPost("{id}/needs")]
    public async Task<IActionResult> AddNeed(int id, [FromBody] AddNeedModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var need = await _locationService.AddNeedAsync(id, model.Category, model.Description, model.Quantity);
            return CreatedAtAction(nameof(GetLocationNeeds), new { id = need.Id }, need);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding need for location ID: {id}");
            return StatusCode(500, new { message = "An error occurred while adding the need" });
        }
    }

    [HttpGet("{id}/needs")]
    public async Task<IActionResult> GetLocationNeeds(int id)
    {
        try
        {
            var needs = await _locationService.GetLocationNeedsAsync(id);
            return Ok(needs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving needs for location ID: {id}");
            return StatusCode(500, new { message = "An error occurred while retrieving location needs" });
        }
    }

    [HttpPost("needs/{needId}/donations")]
    [Authorize]
    public async Task<IActionResult> OfferDonation(int needId, [FromBody] OfferDonationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var donation = await _locationService.OfferDonationAsync(userId, needId, model.Description, model.Quantity, model.Condition, model.ContactInfo);
            return CreatedAtAction(nameof(GetDonationsForNeed), new { needId = donation.NeedId }, donation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error offering donation for need ID: {needId}");
            return StatusCode(500, new { message = "An error occurred while offering the donation" });
        }
    }

    [HttpGet("needs/{needId}/donations")]
    public async Task<IActionResult> GetDonationsForNeed(int needId)
    {
        try
        {
            var donations = await _locationService.GetDonationsForNeedAsync(needId);
            return Ok(donations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving donations for need ID: {needId}");
            return StatusCode(500, new { message = "An error occurred while retrieving donations" });
        }
    }

    [HttpGet("user/donations")]
    [Authorize]
    public async Task<IActionResult> GetUserDonations()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var donations = await _locationService.GetUserDonationsAsync(userId);
            return Ok(donations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user donations");
            return StatusCode(500, new { message = "An error occurred while retrieving user donations" });
        }
    }

    [HttpPut("donations/{donationId}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateDonationStatus(int donationId, [FromBody] UpdateDonationStatusModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedDonation = await _locationService.UpdateDonationStatusAsync(donationId, model.NewStatus);
            if (updatedDonation == null)
            {
                return NotFound();
            }

            await _emailService.SendDonationStatusUpdateEmailAsync(updatedDonation.User.Email, updatedDonation.Status);
            return Ok(updatedDonation);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating status for donation ID: {donationId}");
            return StatusCode(500, new { message = "An error occurred while updating the donation status" });
        }
    }
}

public class LocationFilterModel
{
    public string? Keyword { get; set; }
    public string? Category { get; set; }
    public DateTime? FromDate { get; set; }
}

public class AddLocationModel
{
    [Required] [Range(-90, 90)] public double Latitude { get; set; }

    [Required] [Range(-180, 180)] public double Longitude { get; set; }

    [Required] [StringLength(100)] public string Name { get; set; }
    [Required] [StringLength(100)] public string Description { get; set; }
    [Required] [StringLength(100)] public string Category { get; set; }
}

public class UpdateLocationDetailsModel
{
    [Required] [StringLength(1000)] public string Description { get; set; }

    [Required] [StringLength(50)] public string Category { get; set; }

    public List<IFormFile> Images { get; set; }
}

public class AddFeedbackModel
{
    [Required] [StringLength(1000)] public string Comment { get; set; }

    [Required] [Range(1, 5)] public int Rating { get; set; }
}

public class AddNeedModel
{
    [Required] public string Category { get; set; }

    [Required] public string Description { get; set; }

    [Required] [Range(1, int.MaxValue)] public int Quantity { get; set; }
}

public class OfferDonationModel
{
    [Required] public string Description { get; set; }

    [Required] [Range(1, int.MaxValue)] public int Quantity { get; set; }
    
    [Required] public string Condition { get; set; }
    
    [Required] public string ContactInfo { get; set; }
}

public class UpdateDonationStatusModel
{
    [Required] public string NewStatus { get; set; }
}