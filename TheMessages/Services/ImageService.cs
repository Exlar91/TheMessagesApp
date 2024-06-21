
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace TheMessages.Services
{
    public class ImageService : IImageService
    {
        public async Task<string> LoadImage(IFormFile filename)
        {
            
            var fileName = Guid.NewGuid().ToString() + ".jpeg";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Avatars", fileName);


            using (var memoryStream = new MemoryStream())
            {
                await filename.CopyToAsync(memoryStream);

                // Открытие изображения с помощью ImageSharp
                using (var image = Image.Load(memoryStream.ToArray()))
                {
                    int width = image.Width;
                    int height = image.Height;
                    var rel = 600 / width;

                    var newWidth = width * rel;
                    var newHeight = height * rel;

                    image.Mutate(x => x.Resize(50, 50));
                    // Кодирование изображения обратно в поток
                    memoryStream.SetLength(0);
                    image.SaveAsync (memoryStream, new JpegEncoder()); // Или 

                    using (var filestream = new FileStream(filePath, FileMode.Create))
                    {
                        await filestream.WriteAsync(memoryStream.ToArray());
                    }
                    
                }
            }
            return fileName;
        }
    }
}
