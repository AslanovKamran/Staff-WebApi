using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StaffWebApi.Models.Domain
{
	public class Person
	{
		[Key]
		public int Id { get; set; }

		[Required(AllowEmptyStrings =false)]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false)]
		[MaxLength(100)]
		public string Surname { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false)]
		[MaxLength(255)]
		[Phone]
		public string Phone { get; set; } = string.Empty;

		[Required(AllowEmptyStrings =false)]
		[MaxLength(255)]
		public string Email { get; set; } = string.Empty;

		[MaxLength(255)]
		public string? ImageUrl { get; set; }

		public int PositionId { get; set; }

		public Position Position { get; set; } = new();

    }
}
