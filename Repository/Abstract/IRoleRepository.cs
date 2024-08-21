using StaffWebApi.Models.Domain;

namespace StaffWebApi.Repository.Abstract;

public interface IRoleRepository
{
	Task<List<Role>> GetRolesAsync();
	Task<Role> GetRoleByIdAsync(int id);

	Task<Role> AddRoleAsync(Role role);
	Task<Role> UpdateRoleAsync(Role role);
	Task DeleteRoleByIdAsync(int id);
}
