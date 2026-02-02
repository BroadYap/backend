public class MySQLRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MySQLConnection _dbContext;

    public MySQLRefreshTokenRepository(MySQLConnection dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        using var connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO refresh_login_tokens (id, user_id, token_hash, expires_at, disabled, created_at)
            VALUES (@Id, @UserId, @TokenHash, @ExpiresAt, @Disabled, @CreatedAt)";
        command.Parameters.AddWithValue("@Id", refreshToken.Id);
        command.Parameters.AddWithValue("@UserId", refreshToken.UserId);
        command.Parameters.AddWithValue("@TokenHash", refreshToken.TokenHash);
        command.Parameters.AddWithValue("@ExpiresAt", refreshToken.ExpiresAt);
        command.Parameters.AddWithValue("@Disabled", refreshToken.Disabled);
        command.Parameters.AddWithValue("@CreatedAt", refreshToken.CreatedAt);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string token)
    {
        using var connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT id, user_id, token_hash, expires_at, disabled, created_at
            FROM refresh_login_tokens
            WHERE token_hash = @TokenHash";
        command.Parameters.AddWithValue("@TokenHash", token);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new RefreshToken
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                TokenHash = reader.GetString(2),
                ExpiresAt = reader.GetDateTime(3),
                Disabled = reader.GetBoolean(4),
                CreatedAt = reader.GetDateTime(5)
            };
        }

        throw new InvalidOperationException("Refresh token not found.");
    }

    public async Task DisableRefreshTokenAsync(string token)
    {
        using var connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE refresh_login_tokens
            SET disabled = @Disabled
            WHERE token_hash = @TokenHash";
        command.Parameters.AddWithValue("@Disabled", true);
        command.Parameters.AddWithValue("@TokenHash", token);

        await command.ExecuteNonQueryAsync();
    }

}