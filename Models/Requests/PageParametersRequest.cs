namespace StaffWebApi.Models.Requests;

public class PageParametersRequest
{
	public int ItemsPerPage { get; set; } = 10;

	public int CurrentPage { get; set; } = 1;
}
