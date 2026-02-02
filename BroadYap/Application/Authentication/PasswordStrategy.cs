using System.Security.Authentication;

public class PasswordStrategy : IAuthenticationStrategy
{
    public string ProviderName => "password";

    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher _passwordHasher;

    public PasswordStrategy(IUserRepository userRepository, PasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }
    public async Task<User> AuthenticateAsync(string providerId, string password)
    {
        var user = await _userRepository.GetUserByEmailAsync(providerId);
        if (user == null) 
        {
            throw new AuthenticationException("User not found.");
        }

        if (!_passwordHasher.VerifyPassword(password, user.HashedPassword))
        {
            throw new AuthenticationException("Invalid password.");
        }
        return user;
    }
}