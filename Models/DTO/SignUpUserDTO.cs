using System.ComponentModel.DataAnnotations;

namespace StaffWebApi.Models.DTO
{
	public class SignUpUserDTO
	{
		[Required(AllowEmptyStrings = false)]
		[MaxLength(100)]
		public string Login { get; set; } = string.Empty;

		[Required(AllowEmptyStrings = false)]
		[MaxLength(255)]
		public string Password { get; set; } = string.Empty;

		[Required]
		public int RoleId { get; set; }
	}
}
