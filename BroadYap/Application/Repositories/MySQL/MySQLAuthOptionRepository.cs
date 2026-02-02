public class MySQLAuthOptionRepository : IAuthOptionRepository
{
    //Use connection factory later instead
    private readonly MySQLConnection _dbContext;

    public MySQLAuthOptionRepository(MySQLConnection dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task CreateAuthenticationOptionAsync(AuthenticationOption authOption)
    {
        using var connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO authentication_option (id, provider_name, provider_id, user_id, created_at) VALUES (@Id, @ProviderName, @ProviderId, @UserId, @CreatedAt)";
        command.Parameters.AddWithValue("@Id", authOption.Id);
        command.Parameters.AddWithValue("@ProviderName", authOption.ProviderName);
        command.Parameters.AddWithValue("@ProviderId", authOption.ProviderId);
        command.Parameters.AddWithValue("@UserId", authOption.UserId);
        command.Parameters.AddWithValue("@CreatedAt", authOption.CreatedAt);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<AuthenticationOption?> GetByProviderNameAndIdAsync(string providerName, string providerId)
    {
        using var connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, provider_name, provider_id, user_id, created_at FROM authentication_option WHERE provider_name = @ProviderName AND provider_id = @ProviderId";
        command.Parameters.AddWithValue("@ProviderName", providerName);
        command.Parameters.AddWithValue("@ProviderId", providerId);

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new AuthenticationOption
            {
                Id = reader.GetGuid(0),
                ProviderName = reader.GetString(1),
                ProviderId = reader.GetString(2),
                UserId = reader.GetGuid(3),
                CreatedAt = reader.GetDateTime(4)
            };
        }
        return null;
    }

}