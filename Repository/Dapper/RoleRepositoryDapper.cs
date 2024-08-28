using StaffWebApi.Repository.Abstract;
using System.Data.SqlClient;
using StaffWebApi.Models.Domain;
using System.Data;
using Dapper;

namespace StaffWebApi.Repository.Dapper
{
	public class RoleRepositoryDapper : IRoleRepository
	{

		private readonly string _connectionString;

		public RoleRepositoryDapper(string connectionString) => _connectionString = connectionString;

		public async Task<List<Role>> GetRolesAsync()
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var query = @"exec GetRoles";
				return (await db.QueryAsync<Role>(query)).ToList();
			}
		}

		public async Task<Role> GetRoleByIdAsync(int id)
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

				string query = @"exec GetRoleById @Id";
				var role = await db.QueryFirstOrDefaultAsync<Role>(query, parameters);
				return role!;
			}
		}


		public async Task<Role> AddRoleAsync(Role role)
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("Name", role.Name, DbType.String, ParameterDirection.Input);

				string query = @"exec AddRole @Name";
				try
				{
					var insertedRole= await db.QuerySingleOrDefaultAsync<Role>(query, parameters);
					return insertedRole!;

				}
				catch (SqlException ex)
				{
					throw new InvalidOperationException($"Error while adding a role: {ex.Message}");
				}
			}
		}


		public async Task<Role> UpdateRoleAsync(Role role)
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("Id", role.Id, DbType.Int32, ParameterDirection.Input);
				parameters.Add("Name", CapitalizeFirstLetter(role.Name), DbType.String, ParameterDirection.Input);

				string query = @"exec UpdateRole @Id, @Name";
				var updatedRole= await db.QuerySingleOrDefaultAsync<Role>(query, parameters);
				return updatedRole!;
			}
		}


		public async Task DeleteRoleByIdAsync(int id)
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

				string query = @"DeleteRoleById @Id";
				await db.ExecuteAsync(query, parameters);
			}
		}

		private static string CapitalizeFirstLetter(string input) => char.ToUpper(input[0]) + input[1..].ToLower();

	}
}
