using SixLabors.ImageSharp;

namespace ForumApi.Services.Interfaces
{
    public interface IImageService
    {
        Image Load(IFormFile file);
        void Resize(Image image, int width, int height);
        void ResizeWithAspect(Image image, int width, int height);
        void Crop(Image image);
        Task SaveImage(Image image, string path);
    }
}