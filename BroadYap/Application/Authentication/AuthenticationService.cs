public class AuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthOptionRepository _authOptionRepository;
    private readonly IEnumerable<IAuthenticationStrategy> _authenticationStrategies;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthenticationService(
        IUserRepository userRepository,
        IAuthOptionRepository authOptionRepository,
        IEnumerable<IAuthenticationStrategy> authenticationStrategies,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _authOptionRepository = authOptionRepository;
        _authenticationStrategies = authenticationStrategies;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<RefreshTokenDto> SignUpAsync(SignUpDto signUpParams)
    {
        if (signUpParams == null || string.IsNullOrWhiteSpace(signUpParams.Email) || 
            string.IsNullOrWhiteSpace(signUpParams.Password) || string.IsNullOrWhiteSpace(signUpParams.UserName))
        {
            throw new ArgumentException("Email, password, and username are required.");
        }

        if (signUpParams.Password.Length < 6)
        {
            throw new ArgumentException("Password must be at least 6 characters long.");
        }

        var normalizedEmail = signUpParams.Email.ToLower().Trim();
        var existingUser = await _userRepository.GetUserByEmailAsync(normalizedEmail);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Invalid sign-up parameters.");
        }

        var passwordHasher = new PasswordHasher();
        var hashedPassword = passwordHasher.HashPassword(signUpParams.Password);

        var newUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = signUpParams.UserName.Trim(),
            Email = normalizedEmail,
            HashedPassword = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddUserAsync(newUser);

        var authOption = new AuthenticationOption
        {
            Id = Guid.NewGuid(),
            UserId = newUser.Id,
            ProviderName = "password",
            ProviderId = normalizedEmail,
            CreatedAt = DateTime.UtcNow
        };

        await _authOptionRepository.CreateAuthenticationOptionAsync(authOption);
        
        var token = GenerateSecureToken();
        var tokenHasher = new TokenHasher();
        var hashedToken = tokenHasher.HashToken(token);
        
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = newUser.Id,
            TokenHash = hashedToken,
            ExpiresAt = DateTime.UtcNow.AddDays(3),
            Disabled = false,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateRefreshTokenAsync(refreshToken);

        return new RefreshTokenDto(token, refreshToken.ExpiresAt);
    }

    public async Task<RefreshTokenDto> SignInAsync(SignInDto signInParams)
    {
        //No other options yet just hardcoded we expect they login by password
        var user = await new PasswordStrategy(_userRepository, new PasswordHasher()).AuthenticateAsync(signInParams.Email, signInParams.Password);
        
        var token = Guid.NewGuid().ToString();
        var hashedToken  = new TokenHasher().HashToken(token);

        var refreshToken = new RefreshToken
        {
            Id =  Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = hashedToken,
            ExpiresAt = DateTime.UtcNow.AddDays(3),
            Disabled = false,
            CreatedAt = DateTime.UtcNow
        };

        await _refreshTokenRepository.CreateRefreshTokenAsync(refreshToken);

        return new RefreshTokenDto(token, refreshToken.ExpiresAt);
    }

    private string GenerateSecureToken()
    {
        var randomBytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }
}