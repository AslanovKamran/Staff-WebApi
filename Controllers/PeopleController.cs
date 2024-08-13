using Microsoft.AspNetCore.Mvc;
using StaffWebApi.Models.Domain;
using StaffWebApi.Models.DTO;
using StaffWebApi.Repository.Abstract;

namespace StaffWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PeopleController : ControllerBase
	{
		private readonly IPersonRepository _repository;
		public PeopleController(IPersonRepository repository) => _repository = repository;

		[HttpGet]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetPeople()
		{
			try
			{
				var people = await _repository.GetPeopleAsync();
				if (people == null) return NotFound("No people found.");

				return Ok(people);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
			}
		}

		[HttpGet("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> GetPersonById(int id)
		{
			try
			{
				var person = await _repository.GetPersonByIdAsync(id);

				return person == null ? NotFound($"Person with Id = {id} not found.")
									  : Ok(person);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
			}
		}

		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		public async Task<IActionResult> PostPerson([FromForm] AddPersonDTO dto)
		{
			var person = new Person()
			{
				Name = dto.Name,
				Surname = dto.Surname,
				Phone = dto.Phone,
				Email = dto.Email,
				ImageUrl = dto.ImageUrl,
				PositionId = dto.PositionId
			};
			try
			{

				var addded = await _repository.AddPersonAsync(person);
				return Ok(addded);
			}
			catch (Exception ex)
			{

				return BadRequest(ex.Message);
			}
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(404)]
		[ProducesResponseType(204)]
		public async Task<IActionResult> DeletePerson(int id)
		{
			var person = await _repository.GetPersonByIdAsync(id);
			if (person is null)
			{
				return NotFound("Person with this Id has not been found");
			}
			await _repository.DeletePersonByIdAsync(id);
			return NoContent();

		}

		//update


	}
}
