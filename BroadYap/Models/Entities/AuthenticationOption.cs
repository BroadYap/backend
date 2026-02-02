public class AuthenticationOption
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string ProviderId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}