using Microsoft.AspNetCore.Authorization;
using StaffWebApi.Repository.Abstract;
using StaffWebApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace StaffWebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RolesController : ControllerBase
	{
		private readonly IRoleRepository _repository;
		public RolesController(IRoleRepository repository) => _repository = repository;

		/// <summary>
		/// Get All Roles
		/// </summary>
		/// <returns></returns>

		[HttpGet]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetRolesAsync()
		{
			try
			{

				var roles = await _repository.GetRolesAsync();
				if (roles == null || !roles.Any())
					return NotFound("No roles found");

				return Ok(roles);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
			}
		}

		/// <summary>
		/// Get The Specific Role By Its Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>

		[HttpGet("{id}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(500)]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetRoleById(int id)
		{
			try
			{
				var role = await _repository.GetRoleByIdAsync(id);
				return role == null ? NotFound("Incorrect Position Id") : Ok(role);

			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
			}
		}

		/// <summary>
		/// Add A New Role To The Database (Do not add the same roles)
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>

		[HttpPost]
		[ProducesResponseType(201)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> AddRole([FromForm] Role role)
		{
			if (role == null) return BadRequest("Role data required");
			if (!ModelState.IsValid) return BadRequest(ModelState);

			try
			{
				var addedRole = await _repository.AddRoleAsync(role);

				if (addedRole == null)
					return StatusCode(500, "An error occurred while adding the position.");

				return CreatedAtAction(nameof(AddRole), new { id = addedRole.Id }, addedRole);
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
		/// Update A Role
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>

		[HttpPut]
		[ProducesResponseType(200)]
		[ProducesResponseType(400)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateRole([FromForm] Role role)
		{
			if (role == null) return BadRequest("Position data is required.");

			if (!ModelState.IsValid) return BadRequest(ModelState);

			try
			{
				var updatedRole = await _repository.UpdateRoleAsync(role);
				return Ok(updatedRole);
			}
			catch (SqlException ex)
			{
				return Conflict($"Database error occured while updating a role: {ex.Message}");
			}

			catch (Exception ex)
			{
				return StatusCode(500, $"Unexpected occured while updating a role: {ex.Message}");
			}
		}

		/// <summary>
		/// Delete A Specific Role By Its Id (Do not delete a role referenced by other users)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>

		[HttpDelete]
		[ProducesResponseType(204)]
		[ProducesResponseType(401)]
		[ProducesResponseType(403)]
		[ProducesResponseType(404)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeletePosition(int id)
		{
			if (await _repository.GetRoleByIdAsync(id) is null) 
				return NotFound("No Position Found");

			try
			{
				await _repository.DeleteRoleByIdAsync(id);
				return NoContent();
			}

			catch (SqlException ex)
			{

				return Conflict($"Cannot delete this role: {ex.Message}\n{ex.ErrorCode}");

			}

			catch (Exception ex)
			{
				return StatusCode(500, $"Unexpected occured while deleting a role: {ex.Message}");
			}
		}
	}
}
