namespace StaffWebApi.Helpers;

public static class FileEraser
{
	public static void DeleteImage(string imageName)
	{
		// Construct the file path using Path.Combine
		var path = Path.Combine("wwwroot", "Content", imageName);

		// Check if the file exists before attempting to delete it
		if (File.Exists(path))
		{
			try
			{
				File.Delete(path);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("File deleting failed", ex);
			}
		}
	}
}
