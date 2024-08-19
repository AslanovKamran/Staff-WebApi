namespace StaffWebApi.Helpers;

public static class FileUploader
{
	static public string UploadFile(IFormFile image)
	{

		if (image.Length > 5 * 1024 * 1024) // Limit file size to 5MB
		{
			throw new InvalidOperationException("File size exceeds the limit");
		}

		var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
		var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
		if (!allowedExtensions.Contains(fileExtension))
		{
			throw new InvalidOperationException("Invalid file type");
		}

		var imageName = $"{Guid.NewGuid().ToString()}{image.FileName}";
		var path = Path.Combine("wwwroot", "Content", imageName);

		try
		{
			using (var fs = new FileStream(path, FileMode.Create))
			{
				image.CopyTo(fs);
			}
			return imageName;
		}
		catch (Exception ex)
		{
			// Log the exception or handle it as needed
			throw new InvalidOperationException("File upload failed", ex);
		}
	}
}
