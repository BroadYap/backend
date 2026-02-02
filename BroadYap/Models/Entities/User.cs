public class User 
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; } = true;
    public string HashedPassword { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}