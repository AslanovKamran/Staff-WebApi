using System.Security.Cryptography;
using System.Text;

namespace StaffWebApi.Helpers
{
	public static class PasswordHasher
	{
		public static string HashPassword(string password, string salt)
		{
			var combinedPassword = password + salt;

			using (var pbkdf2 = new Rfc2898DeriveBytes(combinedPassword, Encoding.UTF8.GetBytes(salt), 10000))
			{
				var hashedPasswordBytes = pbkdf2.GetBytes(32);
				return Convert.ToBase64String(hashedPasswordBytes);
			}
		}

		public static string GenerateSalt()
		{
			var saltBytes = new byte[16];
			RandomNumberGenerator.Fill(saltBytes);  
			return Convert.ToBase64String(saltBytes);
		}
	}
}
