using System.ComponentModel.DataAnnotations;

namespace StaffWebApi.Models.DTO;

public class LogInUserDTO
{
	[Required(AllowEmptyStrings = false)]
	[MaxLength(100)]
	public string Login { get; set; } = string.Empty;

	[Required(AllowEmptyStrings = false)]
	[MaxLength(255)]
	public string Password { get; set; } = string.Empty;
}
