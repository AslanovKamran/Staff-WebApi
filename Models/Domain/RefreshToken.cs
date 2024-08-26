namespace StaffWebApi.Models.Domain
{
	public class RefreshToken
	{
		public int Id { get; set; }

		public string Token { get; set; } = string.Empty;

		public int UserId { get; set; }

		public DateTimeOffset Expires { get; set; }

		// Navigation Property
		public User User { get; set; } = new();
	}
}
