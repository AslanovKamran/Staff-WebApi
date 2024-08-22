using System.ComponentModel.DataAnnotations;

namespace StaffWebApi.Models.Domain;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required(AllowEmptyStrings =false)]
    [MaxLength(100)]
    public string Login{ get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    [MaxLength(255)]
    public string Password { get; set; } = string.Empty;

    [Required(AllowEmptyStrings =false)]
    [MaxLength(255)]
    public string Salt{ get; set; } = string.Empty ;

    [Required]
    public int RoleId{ get; set; }

    public DateTime CreatedAt{ get; set; }
    public DateTime? UpdatedAt{ get; set; }

    //Navigation Property
    public Role Role { get; set; } = new();

}
