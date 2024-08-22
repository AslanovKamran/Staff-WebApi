using System.ComponentModel.DataAnnotations;

namespace StaffWebApi.Models.Domain;

public class Role
{
	[Key]
	public int Id { get; set; }

	[Required(AllowEmptyStrings =false)]
	[MaxLength(100, ErrorMessage = "The Title field cannot be empty and exceed 100 characters.")]
	public string Name { get; set; } = string.Empty;

    
}
