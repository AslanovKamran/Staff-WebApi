using StaffWebApi.Models.Domain;

namespace StaffWebApi.Repository.Abstract;

public interface IUserRepository
{
	Task <List<User>> GetUsersAsync();
	Task <User> GetUserByIdAsync(int id);

	Task<User> SignUpUserAsync(User user);
	Task<User> LogInUserAsync(string login);

	Task AddRefreshTokenAsync(RefreshToken refreshToken);
	Task<RefreshToken> GetRefreshTokenAsync(string token);

	Task DeleteRefreshTokenAsync(string token);
	Task DeleteUserRefreshTokensAsync(int id);
}
