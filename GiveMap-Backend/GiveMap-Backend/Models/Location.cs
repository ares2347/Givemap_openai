using System.ComponentModel.DataAnnotations.Schema;

namespace GiveMap_Backend.Models;

public class Location
{
    public int Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public List<string> ImageUrls { get; set; } = new();
    public int UserId { get; set; }
    public User User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}