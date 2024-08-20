namespace GiveMap_Backend.Models;

public class Need
{
    public int Id { get; set; }
    public int LocationId { get; set; }
    public Location Location { get; set; }
    public string Category { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}