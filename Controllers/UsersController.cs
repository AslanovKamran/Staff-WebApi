using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StaffWebApi.Helpers;
using StaffWebApi.Models.Domain;
using StaffWebApi.Models.DTO;
using StaffWebApi.Repository.Abstract;

namespace StaffWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IUserRepository _repository;

	public UsersController(IUserRepository repository) => _repository = repository;

	/// <summary>
	/// Get All Users
	/// </summary>
	/// <returns></returns>

	[HttpGet]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	[ProducesResponseType(500)]
	public async Task<IActionResult> GetUsers()
	{
		try
		{
			var users = await _repository.GetUsersAsync();
			if (users == null || !users.Any())
				return NotFound("No users found");
			return Ok(users);
		}
		catch (Exception ex)
		{
			return 
				StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while processing your request.\n{ex.Message}");
		}
	}

	[HttpGet("{id}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	[ProducesResponseType(500)]
	public async Task<IActionResult> GetUserById(int id) 
	{
		try
		{
			var user = await _repository.GetUserByIdAsync(id);

			return user == null ? NotFound($"User with Id = {id} not found.")
								  : Ok(user);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
		}
	}



	[HttpPost]
	[Route("sign-up")]
	[ProducesResponseType(201)]
	[ProducesResponseType(400)]
	[ProducesResponseType(409)]
	[ProducesResponseType(500)]
	public async Task<IActionResult> SignUpUser([FromForm] SignUpUserDTO signUpUserDTO) 
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var newUser = new User()
		{
			Login = signUpUserDTO.Login,
			Password = signUpUserDTO.Password,
			RoleId = signUpUserDTO.RoleId,
		};

		try
		{
			newUser = await _repository.SignUpUserAsync(newUser);
			if (newUser == null)
				return StatusCode(StatusCodes.Status500InternalServerError, "Sign up proccess failed.");

			var userResponse = new UserRequestResponse
												(id: newUser.Id,
												login: newUser.Login,
												roleId: newUser.RoleId,
												role: newUser.Role,
												createdAt: newUser.CreatedAt,
												updatedAt: newUser.UpdatedAt);

			return CreatedAtAction(nameof(GetUserById), new { id = userResponse.Id }, userResponse);
		}
		catch (SqlException ex)
		{
			return Conflict($"SQL Exception occured. Operation terminated with the code: {ex.ErrorCode}\nError Message: {ex.Message}");
		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
		}

	}


	[HttpPost]
	[Route("log-in")]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	public async Task<IActionResult> LogInUser([FromForm] LogInUserDTO logInUserDTO) 
	{
		if(!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = await _repository.LogInUserAsync(logInUserDTO.Login);
		if (user == null)
			return Unauthorized("Invalid login credentials.");

		var hashedInputPassword = PasswordHasher.HashPassword(logInUserDTO.Password, user.Salt);

		if (hashedInputPassword != user.Password)
			return Unauthorized("Invalid login credentials.");

		var userResponse = new UserRequestResponse
												(id: user.Id,
												login: user.Login,
												roleId: user.RoleId,
												role: user.Role,
												createdAt: user.CreatedAt,
												updatedAt: user.UpdatedAt);
		return Ok(userResponse); // Authentication successful

	}

}
