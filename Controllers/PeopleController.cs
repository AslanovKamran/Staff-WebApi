using StaffWebApi.Repository.Abstract;
using StaffWebApi.Models.Requests;
using StaffWebApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using StaffWebApi.Models.DTO;
using StaffWebApi.Validators;
using System.Data.SqlClient;
using StaffWebApi.Helpers;


namespace StaffWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PeopleController : ControllerBase
{
	private readonly IPersonRepository _repository;

	public PeopleController(IPersonRepository repository) => _repository = repository;

	/// <summary>
	/// Get All People
	/// </summary>
	/// <param name="pageParameters"></param>
	/// <returns></returns>
	
	[HttpGet]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	public async Task<IActionResult> GetPeople([FromQuery] PageParametersRequest pageParameters)
	{		
		const int MaxItemsPerPage = 50;

		if (pageParameters.CurrentPage < 1 || pageParameters.ItemsPerPage < 1)
			return BadRequest("Current Page and Items Per Page must be greater than or equal to 1.");

		try
		{
			var itemsPerPage = Math.Min(pageParameters.ItemsPerPage, MaxItemsPerPage);

			var people = await _repository.GetPeopleAsync(itemsPerPage, pageParameters.CurrentPage);
			if (people == null || !people.Any()) return NotFound("No people found.");
			
			var totalCount = await _repository.GetTotalPeopleCountAsync();
			var pageInfo = new PageInfo(totalCount, itemsPerPage, pageParameters.CurrentPage);

			var result = new
			{
				Data = people,
				PageInfo = pageInfo
			};

			return Ok(result);
		}
		catch (SqlException ex)
		{
			return StatusCode(500, $"Database error: {ex.Message}");
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
		}
	}

	/// <summary>
	/// Get A Person By Their Id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	
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

	/// <summary>
	/// Add A New Person (Login must be unique)
	/// </summary>
	/// <param name="dto"></param>
	/// <param name="image"></param>
	/// <returns></returns>
	
	[HttpPost]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(409)]
	[ProducesResponseType(500)]
	public async Task<IActionResult> AddPerson([FromForm] AddPersonDTO dto, IFormFile? image)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);


		if (!ValidationUtils.IsValidEmail(dto.Email))
			return BadRequest(ValidationUtils.InvalidEmailMessage(dto.Email));


		var person = new Person
		{
			Name = dto.Name,
			Surname = dto.Surname,
			Phone = dto.Phone,
			Email = dto.Email,
			PositionId = dto.PositionId
		};

		if (image != null)
		{
			try
			{
				var imageName = FileUploader.UploadFile(image);
				person.ImageUrl = imageName;

			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");

			}
		}


		try
		{
			var addedPerson = await _repository.AddPersonAsync(person);

			if (addedPerson == null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, "Failed to add the person.");
			}

			return CreatedAtAction(nameof(GetPersonById), new { id = addedPerson.Id }, addedPerson);
		}
		catch (SqlException ex)
		{
			return Conflict($"SQL Exception: A person with the same phone number or email already exists.\n Error Code: {ex.ErrorCode}\nError Message: {ex.Message}");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
		}
	}


	/// <summary>
	/// Update A Person
	/// </summary>
	/// <param name="dto"></param>
	/// <param name="image"></param>
	/// <returns></returns>
	
	[HttpPut]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	[ProducesResponseType(409)]
	[ProducesResponseType(500)]
	public async Task<IActionResult> UpdatePerson([FromForm] UpdatePersonDTO dto, IFormFile? image)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		if (!ValidationUtils.IsValidEmail(dto.Email))
			return BadRequest(ValidationUtils.InvalidEmailMessage(dto.Email));

		var currentPerson = await _repository.GetPersonByIdAsync(dto.Id);
		if (currentPerson == null)
		{
			return NotFound("Person not found.");
		}

		var person = new Person()
		{
			Id = dto.Id,
			Name = dto.Name,
			Surname = dto.Surname,
			Phone = dto.Phone,
			Email = dto.Email,
			PositionId = dto.PositionId,
		};

		if (image != null)
		{
			try
			{
				if (!string.IsNullOrEmpty(currentPerson.ImageUrl))
				{
					FileEraser.DeleteImage(currentPerson.ImageUrl);
				}
				var imageName = FileUploader.UploadFile(image);
				person.ImageUrl = imageName;

			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");

			}
		}

		else
		{
			// Keep the existing ImageUrl if no new image is provided
			person.ImageUrl = currentPerson.ImageUrl;
		}
		try
		{
			var updatedPerson = await _repository.UpdatePersonAsync(person);
			return Ok(updatedPerson);
		}
		catch (SqlException ex)
		{
			return Conflict($"SQL Exception:\nError Code: {ex.ErrorCode}\nError Message: {ex.Message}");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
		}

	}

	/// <summary>
	/// Delete A Person By Their Id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	[HttpDelete("{id}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> DeletePerson(int id)
	{

		var currentPerson = await _repository.GetPersonByIdAsync(id);
		
		var rowsAffected = await _repository.DeletePersonByIdAsync(id);
		if (rowsAffected == 0)
			return NotFound("Person could not be deleted, possibly because it was already removed.");

		if (!string.IsNullOrEmpty(currentPerson.ImageUrl))
		{
			FileEraser.DeleteImage(currentPerson.ImageUrl);
		}
		return NoContent();


	}

}


