using StaffWebApi.Models.Domain;

namespace StaffWebApi.Repository.Abstract
{
	public interface IPersonRepository
	{
		Task<List<Person>> GetPeopleAsync();
		Task<Person> GetPersonByIdAsync(int id);

		Task<Person> AddPersonAsync(Person person);
		Task<Person> UpdatePersonAsync(Person person);
		
		Task DeletePersonByIdAsync(int id);

	}
}
