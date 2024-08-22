using StaffWebApi.Models.Domain;

namespace StaffWebApi.Repository.Abstract;

public interface IUserRepository
{
	Task <List<User>> GetUsersAsync();
	Task <User> GetUserByIdAsync(int id);

	Task<User> SignUpUserAsync(User user);
	Task<User> LogInUserAsync(string login);
}
