using StaffWebApi.Models.Domain;

namespace StaffWebApi.Tokens;

public interface ITokenGenerator
{
	string GenerateAccessToken(User user);
	string GenerateRefreshToken();
}
