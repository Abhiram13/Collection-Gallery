using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Webp;
using CollectionGallery.Domain.Models.Services;

namespace CollectionGallery.Infrastructure.Storage.Services;

public class ImageService
{
    /// <summary>
    /// Loads an image from an uploaded file, resizes it by 50%, and returns the result as a byte array in PNG format.
    /// </summary>
    /// <param name="file">The <see cref="IFormFile"/> containing the image data to be processed.</param>
    /// <returns>A byte array representing the resized image in PNG format.</returns>
    public byte[] ResizeImage(IFormFile file, float sizePercentage)
    {
        using MemoryStream memoryStream = new MemoryStream();
        using Stream inputStream = file.OpenReadStream();
        using Image image = Image.Load(inputStream);
        float percentage = sizePercentage;
        int newWidth = (int)(image.Width * percentage);
        int newHeight = (int)(image.Height * percentage);

        image.Mutate(i => i.Resize(new ResizeOptions
        { 
            Size = new Size(newWidth, newHeight),
            Mode = ResizeMode.Max
        }));

        IImageEncoder encoder;

        switch (file.ContentType)
        {
            case FileContentType.PNG: encoder = new PngEncoder(); break;
            case FileContentType.JPEG: encoder = new JpegEncoder(); break;
            case FileContentType.GIF: encoder = new GifEncoder(); break;
            case FileContentType.WEBP: encoder = new WebpEncoder(); break;
            default: encoder = new PngEncoder(); break;
        }

        image.Save(memoryStream, encoder);
        return memoryStream.ToArray();
    }
}