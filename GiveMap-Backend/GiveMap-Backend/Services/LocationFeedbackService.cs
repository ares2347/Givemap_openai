using GiveMap_Backend.Data;
using GiveMap_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace GiveMap_Backend.Services;

public interface ILocationFeedbackService
{
    Task<LocationFeedback> AddFeedbackAsync(int locationId, int userId, string comment, int rating);
    Task<IEnumerable<LocationFeedback>> GetLocationFeedbackAsync(int locationId);
    Task<double> GetAverageRatingAsync(int locationId);
}

public class LocationFeedbackService : ILocationFeedbackService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LocationFeedbackService> _logger;

    public LocationFeedbackService(ApplicationDbContext context, ILogger<LocationFeedbackService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LocationFeedback> AddFeedbackAsync(int locationId, int userId, string comment, int rating)
    {
        var feedback = new LocationFeedback
        {
            LocationId = locationId,
            UserId = userId,
            Comment = comment,
            Rating = rating,
            CreatedAt = DateTime.UtcNow
        };

        _context.LocationFeedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"New feedback added for location ID: {locationId}");
        return feedback;
    }

    public async Task<IEnumerable<LocationFeedback>> GetLocationFeedbackAsync(int locationId)
    {
        return await _context.LocationFeedbacks
            .Where(f => f.LocationId == locationId)
            .Include(f => f.User)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<double> GetAverageRatingAsync(int locationId)
    {
        return await _context.LocationFeedbacks
            .Where(f => f.LocationId == locationId)
            .AverageAsync(f => f.Rating);
    }
}
