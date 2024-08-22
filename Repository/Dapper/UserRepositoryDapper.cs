using Dapper;
using System.Data.SqlClient;
using System.Data;
using StaffWebApi.Models.Domain;
using StaffWebApi.Repository.Abstract;
using Microsoft.AspNetCore.Identity;
using StaffWebApi.Helpers;
using System.Collections.Generic;

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
				parameters.Add("RoleId", user.RoleId, DbType.Int32, ParameterDirection.Input);  // Corrected to DbType.Int32
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
}
