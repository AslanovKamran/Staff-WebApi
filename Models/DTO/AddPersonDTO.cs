using System.ComponentModel.DataAnnotations;

namespace StaffWebApi.Models.DTO
{
	public class AddPersonDTO
	{
		[Required(AllowEmptyStrings = false)]
		[MaxLength(100)]
		public string Name { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false)]
		[MaxLength(100)]
		public string Surname { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false)]
		[MaxLength(255)]
		[Phone]
		public string Phone { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false)]
		[MaxLength(255)]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[MaxLength(255)]
		public string? ImageUrl { get; set; }

		public int PositionId { get; set; }
	}
}
