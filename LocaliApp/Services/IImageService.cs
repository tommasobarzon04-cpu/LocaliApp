using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocaliApp.Services
{
    public interface IImageService
    {
        Task<List<string>> UploadImagesAsync(List<IFormFile> files);
    }
}
