public interface IAuthOptionRepository
{
    Task CreateAuthenticationOptionAsync(AuthenticationOption authOption);
    Task<AuthenticationOption?> GetByProviderNameAndIdAsync(string providerName, string providerId);
}