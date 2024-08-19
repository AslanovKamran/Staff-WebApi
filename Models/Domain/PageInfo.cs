namespace StaffWebApi.Models.Domain;

public class PageInfo
{
	public int TotalCount { get; set; } = default;
	public int ItemsPerPage { get; set; } = default;
	public int CurrentPage { get; set; } = default;
	public int TotalPages { get; set; } = default;

    public bool HasNextPage{ get; set; }
    public bool HasPreviousPage{ get; set; }

	public PageInfo(int totalCount, int itemsPerPage, int currentPage)
	{
		TotalCount = totalCount;
		ItemsPerPage = itemsPerPage;
		CurrentPage = currentPage;

		TotalPages = (int)Math.Ceiling(TotalCount/(double)ItemsPerPage);
		
		HasNextPage = CurrentPage < TotalPages;
		HasPreviousPage = CurrentPage > 1;
	}
}
