using asp_all.Data;
using asp_all.Middleware.Auth.Token;
using asp_all.Models.Api;
using asp_all.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace asp_all.Controllers.Api
{
    [Route("api/product")]
    [ApiController]
    public class ProductController(DataContext dataContext, IStorageService storageService) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IStorageService _storageService = storageService;

        [HttpGet("{id}")]
        public RestResponse GetProduct(String id)
        {
            String authMessage;
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                authMessage = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Name).Value;
            }
            else
            {
                authMessage = HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? string.Empty;
            }

            var product = _dataContext.ShopProducts.FirstOrDefault(p => p.Slug == id || p.Id.ToString() == id);

            return new()
            {
                Meta = new()
                {
                    ServerTime = DateTime.Now.Ticks,
                    Cache = 3600,
                    ResourceId = id,
                    AuthStatus = authMessage,
                    DataType = product == null ? "null" : "object",
                    Path = HttpContext.Request.Path.Value ?? "",
                    Service = "Asp-Shop API"
                },
                Data = product == null ? null : new ShopProductPage
                {
                    Product = new()
                    {
                        Id = product.Id.ToString(),
                        Title = product.Title,
                        Price = product.Price,
                        Stock = product.Stock,
                        Slug = product.Slug,
                        ImageUrl = _storageService.GetPathPrefix() + (product.ImageUrl ?? "no_image.webp"),
                        Rating = null,
                        Discount = 0
                    },
                    Recommended = [.._dataContext
                    .ShopProducts
                    .Where(p => p.Id != product.Id)
                    .OrderBy(_ => Guid.NewGuid())
                    .Take(3)
                    .Select(p => new ShopProductModel {
                        Id = product.Id.ToString(),
                        Title = product.Title,
                        Price = product.Price,
                        Stock = product.Stock,
                        Slug = product.Slug,
                        ImageUrl = _storageService.GetPathPrefix() + (product.ImageUrl ?? "no_image.webp"),
                        Rating = null,
                        Discount = 0
                    })]
                }
            };
        }
    }
}
