namespace GiveMap_Backend.Models;

public class LocationFeedback
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public Location Location { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}