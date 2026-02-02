public interface IUserRepository
{
    Task<bool> IsEmailTakenAsync(string email);
    Task AddUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string email);
}