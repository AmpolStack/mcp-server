namespace Services.Models;

public class Client
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Address? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

}