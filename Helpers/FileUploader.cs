using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace StaffWebApi.Helpers;

public static class FileUploader
{
	static public string UploadFile(IFormFile image)
	{
		const int QUALITY_PERCENATAGE = 75;
		const int MAX_IMAGE_SIZE_KB = 150;

		var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
		var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
		if (!allowedExtensions.Contains(fileExtension))
			throw new InvalidOperationException("Invalid file type");


		var imageName = $"{Guid.NewGuid()}{(image.FileName).ToLowerInvariant().Replace(" ", "-")}";
		var path = Path.Combine("wwwroot", "Content", imageName);

		try
		{
			using (var imageStream = image.OpenReadStream())
			{
				using (var img = Image.Load(imageStream))
				{
					// Set a fixed width and height
					int fixedWidth = 600;
					int fixedHeight = 800;

					img.Mutate(x => x.Resize(new ResizeOptions
					{
						Size = new Size(fixedWidth, fixedHeight),
						Mode = ResizeMode.Crop
					}));

					// Start with an initial quality setting
					int quality = QUALITY_PERCENATAGE;

					using (var outputStream = new MemoryStream())
					{
						do
						{
							outputStream.SetLength(0); // Reset the stream

							// Create a new encoder each time with the updated quality
							var jpegEncoder = new JpegEncoder
							{
								Quality = quality
							};

							img.Save(outputStream, jpegEncoder);

							// Reduce quality for the next iteration if the size exceeds 150 KB
							quality -= 5;

						} while (outputStream.Length > MAX_IMAGE_SIZE_KB * 1024 && quality > 10);

						// Save the final image to file
						using (var finalFileStream = new FileStream(path, FileMode.Create))
						{
							outputStream.Seek(0, SeekOrigin.Begin);
							outputStream.CopyTo(finalFileStream);
						}
					}
				}
			}

			return imageName;
		}
		catch (Exception ex)
		{
			throw new InvalidOperationException("File upload failed", ex);
		}
	}
}
