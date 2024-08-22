using StaffWebApi.Repository.Abstract;
using StaffWebApi.Models.Domain;
using System.Data.SqlClient;
using System.Data;
using Dapper;


namespace StaffWebApi.Repository.Dapper;

public class PersonRepositoryDapper : IPersonRepository
{
	private readonly string _connectionString;
	public PersonRepositoryDapper(string connectionString) => _connectionString = connectionString;


	public async Task<List<Person>> GetPeopleAsync(int itemsPerPage, int currentPage)
	{
		int skip = (currentPage - 1) * itemsPerPage;
		int take = itemsPerPage;

		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			try
			{
				var parameters = new DynamicParameters();
				parameters.Add("Skip", skip, DbType.Int32, ParameterDirection.Input);
				parameters.Add("Take", take, DbType.Int32, ParameterDirection.Input);

				string query = @"exec GetPeople @Skip, @Take";
				var people = (await db.QueryAsync<Person, Position, Person>(
					query,
					(person, position) =>
					{
						person.Position = position;
						return person;
					},
					parameters
				)).ToList();

				return people;
			}
			catch (Exception ex)
			{
				throw new Exception($"An error occurred while fetching people: {ex.Message}");
			}
		}
	}
	public async Task<Person> GetPersonByIdAsync(int id)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

			string query = "exec GetPersonById @Id";

			var person = (await db.QueryAsync<Person, Position, Person>(query, (prs, pos) =>
			{
				prs.Position = pos;
				return prs;
			}, parameters)).FirstOrDefault();

			return person!;
		}
	}

	public async Task<Person> AddPersonAsync(Person person)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Name", person.Name, DbType.String, ParameterDirection.Input);
			parameters.Add("Surname", person.Surname, DbType.String, ParameterDirection.Input);
			parameters.Add("Phone", person.Phone, DbType.String, ParameterDirection.Input);
			parameters.Add("Email", person.Email, DbType.String, ParameterDirection.Input);
			parameters.Add("ImageUrl", person.ImageUrl, DbType.String, ParameterDirection.Input);
			parameters.Add("PositionId", person.PositionId, DbType.Int32, ParameterDirection.Input);

			string query = "exec AddPerson @Name, @Surname, @Phone, @Email, @ImageUrl, @PositionId";

			var added = (await db.QueryAsync<Person, Position, Person>(query, (person, position) =>
			{
				person.Position = position;
				return person;
			}, parameters)).FirstOrDefault();

			return added!;
		}
	}

	public async Task<Person> UpdatePersonAsync(Person person)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Id", person.Id, DbType.Int32, ParameterDirection.Input);
			parameters.Add("Name", person.Name, DbType.String, ParameterDirection.Input);
			parameters.Add("Surname", person.Surname, DbType.String, ParameterDirection.Input);
			parameters.Add("Phone", person.Phone, DbType.String, ParameterDirection.Input);
			parameters.Add("Email", person.Email, DbType.String, ParameterDirection.Input);
			parameters.Add("ImageUrl", person.ImageUrl, DbType.String, ParameterDirection.Input);
			parameters.Add("PositionId", person.PositionId, DbType.Int32, ParameterDirection.Input);

			string query = "exec UpdatePerson @Id, @Name, @Surname, @Phone, @Email, @ImageUrl, @PositionId";





			var updatedPerson = (await db.QueryAsync<Person, Position, Person>(query, (person, position) =>
			{
				person.Position = position;
				return person;
			}, parameters)).FirstOrDefault();

			return updatedPerson!;
		}
	}

	public async Task<int> DeletePersonByIdAsync(int id)
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			var parameters = new DynamicParameters();
			parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

			string query = @"DeletePersonById @Id";

			var rowsAffected = await db.ExecuteAsync(query, parameters);
			return rowsAffected;
		}
	}

	public async Task<int> GetTotalPeopleCountAsync() 
	{
		using (IDbConnection db = new SqlConnection(_connectionString))
		{
			string query = "GetPeopleCount";
			var totalCount = await db.ExecuteScalarAsync<int>(query);
			return totalCount;
		}
	}

}
