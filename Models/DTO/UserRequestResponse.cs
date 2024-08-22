using StaffWebApi.Models.Domain;

namespace StaffWebApi.Models.DTO;

public class UserRequestResponse
{
	public int Id { get; set; }

	public string Login { get; set; } = string.Empty;
	
	public int RoleId { get; set; }

	public DateTime CreatedAt { get; set; }
	public DateTime? UpdatedAt { get; set; }

	public Role Role { get; set; } = new();

	public UserRequestResponse(int id, string login, int roleId, DateTime createdAt, DateTime? updatedAt, Role role)
	{
		Id = id;
		Login = login;
		RoleId = roleId;
		CreatedAt = createdAt;
		UpdatedAt = updatedAt;
		Role = role;
	}
}
