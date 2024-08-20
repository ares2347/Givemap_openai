using GiveMap_Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace GiveMap_Backend.Services;

public interface IReportService
{
    Task<UserActivityReport> GenerateUserActivityReportAsync(DateTime startDate, DateTime endDate);
    Task<LocationDataReport> GenerateLocationDataReportAsync(DateTime startDate, DateTime endDate);
}

public class ReportService : IReportService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReportService> _logger;

    public ReportService(ApplicationDbContext context, ILogger<ReportService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<UserActivityReport> GenerateUserActivityReportAsync(DateTime startDate, DateTime endDate)
    {
        var report = new UserActivityReport
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalUsers = await _context.Users.CountAsync(),
            NewUsers = await _context.Users.CountAsync(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate),
            ActiveUsers = await _context.Users.CountAsync(u => u.LastLoginDate >= startDate && u.LastLoginDate <= endDate)
        };

        _logger.LogInformation($"Generated user activity report from {startDate} to {endDate}");
        return report;
    }

    public async Task<LocationDataReport> GenerateLocationDataReportAsync(DateTime startDate, DateTime endDate)
    {
        var report = new LocationDataReport
        {
            StartDate = startDate,
            EndDate = endDate,
            TotalLocations = await _context.Locations.CountAsync(),
            NewLocations = await _context.Locations.CountAsync(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate),
            LocationsByCategory = await _context.Locations
                .Where(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate)
                .GroupBy(l => l.Category)
                .Select(g => new CategoryCount { Category = g.Key, Count = g.Count() })
                .ToListAsync()
        };

        _logger.LogInformation($"Generated location data report from {startDate} to {endDate}");
        return report;
    }
}

public class UserActivityReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalUsers { get; set; }
    public int NewUsers { get; set; }
    public int ActiveUsers { get; set; }
}

public class LocationDataReport
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalLocations { get; set; }
    public int NewLocations { get; set; }
    public List<CategoryCount> LocationsByCategory { get; set; }
}

public class CategoryCount
{
    public string Category { get; set; }
    public int Count { get; set; }
}