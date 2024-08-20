using GiveMap_Backend.Data;
using GiveMap_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace GiveMap_Backend.Services;

public interface ILocationService
{
    Task<Location> AddLocationAsync(double latitude, double longitude, string name, string description, string category,  int userId);

    Task<IEnumerable<Location>> GetLocationsAsync(string keyword = null, string category = null,
        DateTime? fromDate = null);

    Task<Location> GetLocationDetailsAsync(int locationId);

    Task<Location> UpdateLocationDetailsAsync(int locationId, string description, string category,
        List<string> imageUrls);

    Task<IEnumerable<Location>> SearchLocationsAsync(string keyword);
    Task DeleteLocationAsync(int locationId);
    Task<Need> AddNeedAsync(int locationId, string category, string description, int quantity);
    Task<IEnumerable<Need>> GetLocationNeedsAsync(int locationId);
    Task<Donation> OfferDonationAsync(int userId, int needId, string description, int quantity, string condition, string contactInfo);
    Task<IEnumerable<Donation>> GetDonationsForNeedAsync(int needId);
    Task<IEnumerable<Donation>> GetUserDonationsAsync(int userId);
    Task<Donation> UpdateDonationStatusAsync(int donationId, string newStatus);
}

public class LocationService : ILocationService
{
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<LocationService> _logger;

    public LocationService(ApplicationDbContext context, IEmailService emailService, ILogger<LocationService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<Location> AddLocationAsync(double latitude, double longitude, string name, string description, string category, int userId)
    {
        var location = new Location
        {
            Latitude = latitude,
            Longitude = longitude,
            Name = name,
            Description = description,
            Category = category,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"New location added: {name} at ({latitude}, {longitude})");
        return location;
    }

    public async Task<IEnumerable<Location>> GetLocationsAsync(string keyword = null, string category = null,
        DateTime? fromDate = null)
    {
        var query = _context.Locations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(l => l.Name.Contains(keyword) || l.Description.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(l => l.Category == category);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(l => l.CreatedAt >= fromDate.Value);
        }

        var locations = await query.ToListAsync();

        _logger.LogInformation(
            $"Retrieved locations. Keyword: {keyword}, Category: {category}, FromDate: {fromDate}. Found {locations.Count} results.");
        return locations;
    }

    public async Task<Location> GetLocationDetailsAsync(int locationId)
    {
        var location = await _context.Locations
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.Id == locationId);

        if (location == null)
        {
            _logger.LogWarning($"Location not found for ID: {locationId}");
            return null;
        }

        _logger.LogInformation($"Retrieved details for location ID: {locationId}");
        return location;
    }

    public async Task<Location> UpdateLocationDetailsAsync(int locationId, string description, string category,
        List<string> imageUrls)
    {
        var location = await _context.Locations.FindAsync(locationId);
        if (location == null)
        {
            throw new ArgumentException("Location not found");
        }

        location.Description = description;
        location.Category = category;
        location.ImageUrls = imageUrls;
        location.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation($"Location details updated for location ID: {locationId}");
        return location;
    }

    public async Task<IEnumerable<Location>> SearchLocationsAsync(string keyword)
    {
        var locations = await _context.Locations
            .Where(l => l.Name.Contains(keyword) || l.Description.Contains(keyword) || l.Category.Contains(keyword))
            .ToListAsync();

        _logger.LogInformation($"Searched locations with keyword: {keyword}. Found {locations.Count} results.");
        return locations;
    }

    public async Task DeleteLocationAsync(int locationId)
    {
        var location = await _context.Locations.FindAsync(locationId);
        if (location != null)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Location with ID {locationId} has been deleted.");
        }
    }

    public async Task<Need> AddNeedAsync(int locationId, string category, string description, int quantity)
    {
        var need = new Need
        {
            LocationId = locationId,
            Category = category,
            Description = description,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow
        };

        _context.Needs.Add(need);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"New need added for location ID: {locationId}");
        return need;
    }

    public async Task<IEnumerable<Need>> GetLocationNeedsAsync(int locationId)
    {
        return await _context.Needs
            .Where(n => n.LocationId == locationId)
            .ToListAsync();
    }

    public async Task<Donation> OfferDonationAsync(int userId, int needId, string description, int quantity, string condition, string contactInfo)
    {
        var donation = new Donation
        {
            UserId = userId,
            NeedId = needId,
            Description = description,
            Quantity = quantity,
            Condition = condition,
            ContactInfo = contactInfo,
            CreatedAt = DateTime.UtcNow,
            Status = "Offered"
        };

        _context.Donations.Add(donation);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"New donation offered for need ID: {needId}");
        return donation;
    }

    public async Task<IEnumerable<Donation>> GetDonationsForNeedAsync(int needId)
    {
        return await _context.Donations
            .Where(d => d.NeedId == needId)
            .Include(d => d.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Donation>> GetUserDonationsAsync(int userId)
    {
        return await _context.Donations
            .Where(d => d.UserId == userId)
            .Include(d => d.Need)
            .ThenInclude(n => n.Location)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Donation> UpdateDonationStatusAsync(int donationId, string newStatus)
    {
        var donation = await _context.Donations
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.Id == donationId);

        if (donation != null)
        {
            donation.Status = newStatus;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Updated status for donation ID: {donationId} to {newStatus}");

            // Send email notification to the donor
            await _emailService.SendDonationStatusUpdateEmailAsync(donation.User.Email, newStatus);
        }
        return donation;
    }
}