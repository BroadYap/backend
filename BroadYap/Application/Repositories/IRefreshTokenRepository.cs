public interface IRefreshTokenRepository
{
    Task CreateRefreshTokenAsync(RefreshToken refreshToken);
    Task<RefreshToken> GetRefreshTokenAsync(string token);
    Task DisableRefreshTokenAsync(string token);
}