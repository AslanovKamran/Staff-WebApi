using System.ComponentModel.DataAnnotations;

namespace StaffWebApi.Models.Domain
{
	
	public class Position
	{
		[Key]
		public int Id { get; set; }

		[Required(AllowEmptyStrings = false)]
		[MaxLength(255, ErrorMessage = "The Title field cannot be empty and exceed 255 characters.")]
		public string Title { get; set; } = string.Empty;

		[Required]
		[Range(1, double.MaxValue, ErrorMessage = "The Salary must be a positive number")]
		public decimal Salary { get; set; } = default;
	}
}
