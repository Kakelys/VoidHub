using SixLabors.ImageSharp;

namespace ForumApi.Services.Interfaces
{
    public interface IImageService
    {
        Image Load(IFormFile file);
        void Resize(Image image);
        Task SaveImage(Image image, string path);
    }
}