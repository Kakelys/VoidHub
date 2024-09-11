using ForumApi.Services.Utils.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace ForumApi.Services.Utils;

public class ImageService() : IImageService
{
    public Image Load(IFormFile file)
    {
        return Image.Load(file.OpenReadStream());
    }

    public void Resize(Image image, int width, int height)
    {
        var newWidth = image.Width < width ? image.Width : width;
        var newHeight = image.Height < height ? image.Height : height;

        image.Mutate(x => x.Resize(newWidth, newHeight));
    }

    public void ResizeWithAspect(Image image, int width, int height)
    {
        var newWidth = image.Width < width ? image.Width : width;
        var newHeight = image.Height < height ? image.Height : height;

        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(newWidth, newHeight),
            Mode = ResizeMode.Max
        }));
    }

    public void Crop(Image image)
    {
        var minSize = Math.Min(image.Width, image.Height);
        image.Mutate(x => x.Crop(new Rectangle((image.Width - minSize) / 2, (image.Height - minSize) / 2, minSize, minSize)));
    }

    public async Task SaveImage(Image image, string path)
    {
        var encoder = new PngEncoder()
        {
            CompressionLevel = PngCompressionLevel.DefaultCompression,
            TransparentColorMode = PngTransparentColorMode.Preserve,
            SkipMetadata = true,
        };

        await using var stream = new FileStream(path, FileMode.Create);
        await image.SaveAsync(stream, encoder);
    }
}