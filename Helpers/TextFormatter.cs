namespace StaffWebApi.Helpers
{
	public static class TextFormatter
	{
		public static string ToTitleCase(string input) 
		{
			return char.ToUpper(input[0]) + input[1..].ToLower();
		}
	}
}
