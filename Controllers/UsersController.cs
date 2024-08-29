using Microsoft.AspNetCore.Authorization;
using StaffWebApi.Repository.Abstract;
using StaffWebApi.Models.Requests;
using StaffWebApi.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using StaffWebApi.Models.DTO;
using System.Data.SqlClient;
using StaffWebApi.Helpers;
using StaffWebApi.Tokens;

namespace StaffWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
	private readonly IUserRepository _repository;
	private readonly ITokenGenerator _tokenGenerator;

	public UsersController(IUserRepository repository, ITokenGenerator tokenGenerator)
	{
		_repository = repository;
		_tokenGenerator = tokenGenerator;
	}

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

	/// <summary>
	/// Get User By Id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>

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


	/// <summary>
	/// Sign Up New User (Then proceed to Log In)
	/// </summary>
	/// <param name="signUpUserDTO"></param>
	/// <returns></returns>

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


	/// <summary>
	/// Login In User And Get Access and Refresh Tokens
	/// </summary>
	/// <param name="logInUserDTO"></param>
	/// <returns></returns>

	[HttpPost]
	[Route("log-in")]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	public async Task<IActionResult> LogInUser([FromForm] LogInUserDTO logInUserDTO)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		var user = await _repository.LogInUserAsync(logInUserDTO.Login);
		if (user == null)
			return Unauthorized("Invalid login credentials.");

		var hashedInputPassword = PasswordHasher.HashPassword(logInUserDTO.Password, user.Salt);

		if (hashedInputPassword != user.Password)
			return Unauthorized("Invalid login credentials.");

		//Generating Tokens 
		var accessToken = _tokenGenerator.GenerateAccessToken(user);
		var refreshToken = new RefreshToken
		{
			Token = _tokenGenerator.GenerateRefreshToken(),
			UserId = user.Id,
			Expires = DateTime.Now + TimeSpan.FromDays(31)
		};

		//Add a new RefreshToken to the Data Base
		await _repository.AddRefreshTokenAsync(refreshToken);

		var response = new { AccessToken = accessToken, RefreshToken = refreshToken.Token };
		return Ok(response);

	}


	/// <summary>
	/// Log Out User (Deletes refresh tokens)
	/// </summary>
	/// <param name="userId"></param>
	/// <returns></returns>

	[HttpPost]
	[Route("log-out")]
	[ProducesResponseType(200)]
	[ProducesResponseType(500)]
	public async Task<IActionResult> LogOutUser([FromQuery] int userId)
	{
		try
		{
			await _repository.DeleteUserRefreshTokensAsync(userId);
			return Ok("Users Tokens deleted");

		}
		catch (Exception ex)
		{
			return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
		}
	}

	[HttpPost]
	[Route("refresh")]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	public async Task<ActionResult> Refresh(RefreshRequest request)
	{
		// Validate the refresh token
		if (string.IsNullOrWhiteSpace(request.RefreshToken))
			return BadRequest("Refresh token is required.");

		//Get OldToken (with mapped user) by refreshToken
		var oldToken = await _repository.GetRefreshTokenAsync(request.RefreshToken);
		if (oldToken == null)
			return Unauthorized("Invalid Refresh Token");

		else if (oldToken.Expires < DateTimeOffset.UtcNow)
		{
			//Remove the expired old token
			await _repository.DeleteRefreshTokenAsync(oldToken.Token);
			return Unauthorized("Refresh token expired. Try to log in again");
		}

		var user = oldToken.User;
		if (user == null)
			return Unauthorized("User not found");

		//RemoveOldToken
		await _repository.DeleteRefreshTokenAsync(oldToken.Token);

		//Create new refreshToken 
		RefreshToken newRefreshToken = new()
		{
			Token = _tokenGenerator.GenerateRefreshToken(),
			UserId = user.Id,
			Expires = DateTime.Now + TimeSpan.FromDays(30),
		};

		// Add the new refresh token to the database
		await _repository.AddRefreshTokenAsync(newRefreshToken);
		var newAccessToken = _tokenGenerator.GenerateAccessToken(user);
		var tokens = new { AccessToken = newAccessToken, RefreshToken = newRefreshToken.Token };
		return Ok(tokens);
	}

	[HttpPost]
	[Route("changePassword")]
	[ProducesResponseType(400)]
	[ProducesResponseType(401)]
	[ProducesResponseType(403)]
	[ProducesResponseType(409)]
	[ProducesResponseType(500)]
	[Authorize(Roles ="Admin, User, Guest")]
	public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordRequest request)
	{
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		try
		{
			await _repository.ChangeUserPasswordAsync(request.Login, request.OldPassword, request.NewPassword);
			return Ok(new
			{
				Success = true,
				Message = $"Password of the user {request.Login} has been changed successfully"
			});

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

}
