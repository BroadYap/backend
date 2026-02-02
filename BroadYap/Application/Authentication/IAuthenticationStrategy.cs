public interface IAuthenticationStrategy
{
    string ProviderName { get; }
    Task<User> AuthenticateAsync(string providerId, string secret);
}