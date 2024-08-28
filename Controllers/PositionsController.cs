using Microsoft.AspNetCore.Authorization;
using StaffWebApi.Repository.Abstract;
using StaffWebApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace StaffWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]

public class PositionsController : ControllerBase
{
	private readonly IPositionRepository _repository;
	public PositionsController(IPositionRepository repository) => _repository = repository;


	/// <summary>
	/// Get All Positions
	/// </summary>
	/// <returns></returns>

	[HttpGet]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	[ProducesResponseType(404)]
	[ProducesResponseType(500)]
	[Authorize(Roles = "Admin, User, Guest")]
	public async Task<IActionResult> GetPositions()
	{
		try
		{
			var positions = await _repository.GetPositionsAsync();
			if (positions == null || !positions.Any())
				return NotFound("No positoins found");

			return Ok(positions);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
		}
	}

	/// <summary>
	/// Get A Position By Id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	/// 

	[HttpGet("{id}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	[ProducesResponseType(500)]
	[Authorize(Roles = "Admin, User, Guest")]
	public async Task<IActionResult> GetPositionById(int id)
	{
		try
		{
			var postion = await _repository.GetPositionByIdAsync(id);
			return postion == null ? NotFound("Incorrect Position Id") : Ok(postion);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
		}
	}


	/// <summary>
	/// Add A New Position (Do not add the same position)
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>
	/// 

	[HttpPost]
	[ProducesResponseType(201)]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	[ProducesResponseType(403)]
	[ProducesResponseType(409)]
	[ProducesResponseType(500)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> AddPosition([FromForm] Position position)
	{
		if (position == null) return BadRequest("Position data required");
		if (!ModelState.IsValid) return BadRequest(ModelState);

		try
		{
			var addedPosition = await _repository.AddPositionAsync(position);

			if (addedPosition == null)
			{
				return StatusCode(500, "An error occurred while adding the position.");
			}

			return CreatedAtAction(nameof(AddPosition), new { id = addedPosition.Id }, addedPosition);
		}
		catch (InvalidOperationException ex)
		{
			return Conflict(ex.Message); // Returns HTTP 409 Conflict
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
		}
	}

	/// <summary>
	/// Update A Position
	/// </summary>
	/// <param name="position"></param>
	/// <returns></returns>

	[HttpPut]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	[ProducesResponseType(403)]
	[ProducesResponseType(404)]
	[ProducesResponseType(500)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> UpdatePosition([FromForm] Position position)
	{
		if (position == null)
			return BadRequest("Position data is required.");


		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var existingPosition = await _repository.GetPositionByIdAsync(position.Id);
		if (existingPosition == null)
		{
			return NotFound("Position not found.");
		}

		try
		{
		// Update the position and get the result
		var updatedPosition = await _repository.UpdatePositionAsync(position);

		// Return the updated position
		return Ok(updatedPosition);

		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
		}
	}

	/// <summary>
	/// Delete A Position by Its Id (Don't delete positions referenced by other entities)
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>

	[HttpDelete("{id}")]
	[ProducesResponseType(204)]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	[ProducesResponseType(403)]
	[ProducesResponseType(404)]
	[ProducesResponseType(500)]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeletePosition(int id)
	{
		if (await _repository.GetPositionByIdAsync(id) is null) 
			return NotFound("No Position Found");

		try
		{
			await _repository.DeletePositionByIdAsync(id);
			return NoContent();
		}

		catch (SqlException ex)
		{
			//Reference key constraint exception 
			if (ex.Number == 547)
				return BadRequest(new { message = "Cannot delete this position because it is referenced by one or more people." });

			else
				return StatusCode(500, "Internal server error");
		}

		catch (Exception ex)
		{
			return BadRequest($"Failed to delete position {ex.Message}");
		}
	}

}
