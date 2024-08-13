﻿using System.Data.SqlClient;
using System.Data;
using StaffWebApi.Models.Domain;
using StaffWebApi.Repository.Abstract;
using Dapper;
using System.Numerics;
using System.Xml.Linq;

namespace StaffWebApi.Repository.Dapper
{
	public class PersonRepositoryDapper : IPersonRepository
	{
		private readonly string _connectionString;
		public PersonRepositoryDapper(string connectionString) => _connectionString = connectionString;


		public async Task<List<Person>> GetPeopleAsync()
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				string query = @"exec GetPeople";
				var people = (await db.QueryAsync<Person, Position, Person>(query, (person, position) =>
				{
					person.Position = position;
					return person;
				})).ToList();
				return people;
			}
		}

		public async Task<Person> GetPersonByIdAsync(int id)
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

				string query = "exec GetPersonById @Id";

				var user = (await db.QueryAsync<Person, Position, Person>(query, (person, position) =>
				{
					person.Position = position;
					return person;
				}, parameters)).FirstOrDefault();

				return user!;
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

		public Task<Person> UpdatePersonAsync(Person person)
		{
			throw new NotImplementedException();
		}

		public async Task DeletePersonByIdAsync(int id)
		{
			using (IDbConnection db = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("Id", id, DbType.Int32, ParameterDirection.Input);

				string query = @"DeletePersonById @Id";
				await db.ExecuteAsync(query, parameters);
			}
		}

	}
}