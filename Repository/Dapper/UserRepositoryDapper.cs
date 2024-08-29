using StaffWebApi.Repository.Abstract;
using System.Security.Authentication;
using StaffWebApi.Models.Domain;
using System.Data.SqlClient;
using StaffWebApi.Helpers;
using System.Data;
using Dapper;

namespace StaffWebApi.Repository.Dapper;

public class UserRepositoryDapper : IUserRepository
{
	private readonly string _connectionString;

	public UserRepositoryDapper(string connectionString) => _connectionString = connectionString;

	public async Task<List<User>> GetUsersAsync()
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			try
			{
				string query = @"exec GetUsers";
				var users = (await db.QueryAsync<User, Role, User>(
					query,
					(user, role) =>
					{
						user.Role = role;
						return user;
					}

				)).ToList();

				return users;
			}
			catch (SqlException sqlEx)
			{
				// Handle SQL-specific errors
				throw new Exception($"Database error occurred while fetching users: {sqlEx.Message}", sqlEx);
			}
			catch (Exception ex)
			{
				// Handle general errors
				throw new Exception($"An error occurred while fetching users: {ex.Message}", ex);
			}
		}
	}

	public async Task<User> GetUserByIdAsync(int id)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			try
			{


				var parameters = new DynamicParameters();
				parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

				string query = "exec GetUserById @Id";

				var user = (await db.QueryAsync<User, Role, User>(query, (user, role) =>
				{
					user.Role = role;
					return user;
				}, parameters)).FirstOrDefault();

				return user!;
			}
			catch (SqlException sqlEx)
			{
				// Handle SQL-specific errors
				throw new Exception($"Database error occurred while fetching users: {sqlEx.Message}", sqlEx);
			}
			catch (Exception ex)
			{
				// Handle general errors
				throw new Exception($"An error occurred while fetching users: {ex.Message}", ex);
			}
		}
	}

	public async Task<User> SignUpUserAsync(User user)
	{
		user.Salt = PasswordHasher.GenerateSalt();
		user.Password = PasswordHasher.HashPassword(user.Password, user.Salt);

		try
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("Login", user.Login, DbType.String, ParameterDirection.Input);
				parameters.Add("Password", user.Password, DbType.String, ParameterDirection.Input);
				parameters.Add("RoleId", user.RoleId, DbType.Int32, ParameterDirection.Input);  
				parameters.Add("Salt", user.Salt, DbType.String, ParameterDirection.Input);

				string query = @"exec AddUser @Login, @Password, @RoleId, @Salt";

				var addedUser = (await db.QueryAsync<User, Role, User>(query, (u, role) =>
				{
					u.Role = role;
					return u;
				}, parameters)).FirstOrDefault();

				return addedUser ?? throw new InvalidOperationException("User could not be created.");
			}
		}
		catch (SqlException sqlEx)
		{
			throw new Exception($"Database error occurred while signing up the user: {sqlEx.Message}", sqlEx);
		}
		catch (Exception ex)
		{
			throw new Exception($"An error occurred while signing up the user: {ex.Message}", ex);
		}
	}

	public async Task<User> LogInUserAsync(string login)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Login", login, DbType.String, ParameterDirection.Input);

			string query = @"exec GetUserByLogin @Login";
			var loggedInUser = (await db.QueryAsync<User, Role, User>(
				query,
				(user, role) =>
				{
					user.Role = role;
					return user;
				}, parameters)).FirstOrDefault();

			return loggedInUser!;

		}
	}

	public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("@Token", refreshToken.Token);
			parameters.Add("@Expires", refreshToken.Expires);
			parameters.Add("@UserId", refreshToken.UserId);

			string query = "exec AddRefreshToken @Token, @Expires, @UserId";
			await db.ExecuteAsync(query, parameters);
		}
	}

	public async Task<RefreshToken> GetRefreshTokenAsync(string token)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Token", token, DbType.String, ParameterDirection.Input);

			string query = @"exec GetUserByRefreshToken @Token";
			var result = await db.QueryAsync<RefreshToken, User, Role, RefreshToken>(
			query,
			(refreshToken, user, role) =>
			{
				refreshToken.User = user;
				user.Role = role;
				return refreshToken;
			},
			parameters
		);
			return result.FirstOrDefault()!;
		}
	}

	public async Task DeleteRefreshTokenAsync(string token)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("@Token", token, DbType.String, ParameterDirection.Input);

			string query = @"exec DeleteRefreshToken @Token";
			await db.ExecuteAsync(query, parameters);
		}
	}

	public async Task DeleteUserRefreshTokensAsync(int userId)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("UserId", userId, DbType.Int32, ParameterDirection.Input);

			string query = @"exec DeleteUserRefreshTokens @UserId";
			await db.ExecuteAsync(query, parameters);
		}
	}

	public async Task ChangeUserPasswordAsync(string login, string oldPassword, string newPassword)
	{
		var user = await LogInUserAsync(login) 
			?? throw new InvalidCredentialException(@"Wrong credentials for user: {login}");

		oldPassword = PasswordHasher.HashPassword(oldPassword, user.Salt);
		newPassword = PasswordHasher.HashPassword(newPassword, user.Salt);

		try
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("Login", login, DbType.String, ParameterDirection.Input);
				parameters.Add("OldPassword", oldPassword, DbType.String, ParameterDirection.Input);
				parameters.Add("NewPassword", newPassword, DbType.String, ParameterDirection.Input);

				string query = @"exec  ChangeUserPassword @Login, @OldPassword, @NewPassword";
				await db.ExecuteAsync(query, parameters);
			}
		}
		catch (SqlException sqlEx)
		{

			throw new Exception($"Error in the database: {sqlEx.Message}", sqlEx); ;
		}
		catch (Exception ex)
		{
			throw new Exception($"An error occurred while changing the user password: {ex.Message}", ex);
		}
	}

}
