using CenngeShop.Services.Storage;
using Microsoft.AspNetCore.Mvc;

namespace CenngeShop.Controllers
{
    public class StorageController(IStorageService storageService) : Controller
    {
        private readonly IStorageService _storageService = storageService;

        [HttpGet]
        public IActionResult Item([FromRoute] String id)
        {
            String ext = Path.GetExtension(id);
            String mimeType = ext switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
            try
            {
                return File(_storageService.Load(id), mimeType);
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
