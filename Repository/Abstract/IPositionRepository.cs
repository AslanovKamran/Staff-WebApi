using StaffWebApi.Models.Domain;

namespace StaffWebApi.Repository.Abstract
{
	public interface IPositionRepository
	{
		Task<List<Position>> GetPositionsAsync();
		Task<Position> GetPositionByIdAsync(int id);

		Task<Position> AddPositionAsync(Position position);
		Task DeletePositionByIdAsync(int id);
		Task<Position> UpdatePositionAsync(Position position);
	}
}
