namespace TheMessages.Services
{
    public interface IImageService
    {   
        Task<string> LoadImage(IFormFile file);
    }
}
