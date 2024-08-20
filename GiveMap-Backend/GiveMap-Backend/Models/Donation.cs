namespace GiveMap_Backend.Models;

public class Donation
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int NeedId { get; set; }
    public Need Need { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    
    public string Condition { get; set; }
    
    public string ContactInfo { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
}