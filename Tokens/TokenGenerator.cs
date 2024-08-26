using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using StaffWebApi.Models.Domain;
using System.Security.Claims;

namespace StaffWebApi.Tokens;

public class TokenGenerator : ITokenGenerator
{
	private readonly JwtOptions _options;
	public TokenGenerator(IOptions<JwtOptions> options) => _options = options.Value;

	public string GenerateAccessToken(User user)
	{
		var issuedAt = DateTime.Now;

		//Generating Claims
		var claims = new List<Claim>()
		{
			new ("id", (user.Id).ToString()),
			new ("sub", user.Login),
			new ("role", user.Role.Name),
			new ("iat",ToUnixEpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64)
		};

		//Generating a token
		var token = new JwtSecurityToken(
			issuer: _options.Issuer,
			audience: _options.Audience,
			claims,
			expires: issuedAt + _options.AccessValidFor,
			signingCredentials: _options.SigningCredentials
			);
		return new JwtSecurityTokenHandler().WriteToken(token);

	}

	public string GenerateRefreshToken() => Guid.NewGuid().ToString();

	private static long ToUnixEpochDate(DateTime date)
	{
		var offset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
		return (long)Math.Round((date.ToUniversalTime() - offset).TotalSeconds);
	}
}