public class MySQLUserRepository : IUserRepository
{
    //Use connection factory later instead
    private readonly MySQLConnection _dbContext;

    public MySQLUserRepository(MySQLConnection dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        using var connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, user_name, email, password_hash, is_active, created_at FROM users WHERE email = @Email";
        command.Parameters.AddWithValue("@Email", email.ToLower());

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(0),
                UserName = reader.GetString(1),
                Email = reader.GetString(2),
                HashedPassword = reader.GetString(3),
                IsActive = reader.GetBoolean(4),
                CreatedAt = reader.GetDateTime(5)
            };
        }
        return null;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        var user = await GetUserByEmailAsync(email.ToLower());
        return user != null;
    }

    public async Task AddUserAsync(User user)
    {
        using var connection = _dbContext.CreateConnection();
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO users (id, user_name, email, password_hash, is_active, created_at) VALUES (@Id, @UserName, @Email, @HashedPassword, @IsActive, @CreatedAt)";
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@UserName", user.UserName);
        command.Parameters.AddWithValue("@Email", user.Email.ToLower());
        command.Parameters.AddWithValue("@HashedPassword", user.HashedPassword);
        command.Parameters.AddWithValue("@IsActive", user.IsActive);
        command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

        await command.ExecuteNonQueryAsync();
    }
}