using StaffWebApi.Repository.Abstract;
using StaffWebApi.Models.Domain;
using System.Data.SqlClient;
using System.Data;
using Dapper;


namespace StaffWebApi.Repository.Dapper;

public class PositionRepositoryDapper : IPositionRepository
{
	private readonly string _connectionString = string.Empty;
	public PositionRepositoryDapper(string connectionString) => _connectionString = connectionString;


	public async Task<List<Position>> GetPositionsAsync()
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var query = @"exec GetPositions";
			return (await db.QueryAsync<Position>(query)).ToList();
		}
	}
	public async Task<Position> GetPositionByIdAsync(int id)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

			string query = @"exec GetPositionById @Id";
			var position = await db.QueryFirstOrDefaultAsync<Position>(query, parameters);
			return position!;
		}
	}

	public async Task<Position> AddPositionAsync(Position position)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Title", position.Title, DbType.String, ParameterDirection.Input);
			parameters.Add("Salary", position.Salary, DbType.Decimal, ParameterDirection.Input);


			string query = @"exec AddPosition @Title, @Salary";
			try
			{
				var insertedPosition = await db.QuerySingleOrDefaultAsync<Position>(query, parameters);
				return insertedPosition!;

			}
			catch (SqlException ex)
			{

				throw new InvalidOperationException($"Error while adding a position: {ex.Message}");

			}
		}
	}

	public async Task<Position> UpdatePositionAsync(Position position)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Id", position.Id, DbType.Int32, ParameterDirection.Input);
			parameters.Add("Title", CapitalizeFirstLetter(position.Title), DbType.String, ParameterDirection.Input);
			parameters.Add("Salary", position.Salary, DbType.Decimal, ParameterDirection.Input);

			string query = @"exec UpdatePosition @Id, @Title, @Salary";
			var updatedPosition = await db.QuerySingleOrDefaultAsync<Position>(query, parameters);
			return updatedPosition!;
		}
	}

	public async Task DeletePositionByIdAsync(int id)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

			string query = @"DeletePositionById @Id";
			await db.ExecuteAsync(query, parameters);
		}
	}

	private static string CapitalizeFirstLetter(string input) => char.ToUpper(input[0]) + input[1..].ToLower();
	

}