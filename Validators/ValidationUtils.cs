using System.Text.RegularExpressions;

namespace StaffWebApi.Validators
{
	public static class ValidationUtils
	{
		public static bool IsValidEmail(string email) 
		{

			Regex regex = new(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
						   + "@"
						   + @"((?!-)[\w\-]+(?<!-)\.)+[a-zA-Z]{2,}$");

			Match match = regex.Match(email);
			return match.Success;
		}
		public static string InvalidEmailMessage(string email) => $"Invalid email format try something like valid.email@example.com. Your email is {email}";

		
	}
}
