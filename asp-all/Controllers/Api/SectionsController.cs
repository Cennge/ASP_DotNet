using asp_all.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace asp_all.Controllers.Api
{
    [Route("api/sections")]
    [ApiController]
    public class SectionsController(DataContext dataContext) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private String StorageUrl => $"{Request.Scheme}://{Request.Host}/Storage/Item/";

        [HttpGet]
        public Object DoGet()
        {
            var sections = _dataContext
                .ShopSections
                .Where(s => s.DeletedAt == null)
                .AsEnumerable()
                .Select(s => s with { ImageUrl =  StorageUrl + s.ImageUrl });
            return sections;
        }

        [HttpGet("{id}")]
        public Object? ProductsBySection(String id)
        {
            var section = _dataContext
                .ShopSections
                .Include(s => s.Products)
                .AsNoTracking()
                .FirstOrDefault(s => s.DeletedAt == null && s.Id.ToString() == id || s.Slug == id);
            if (section != null) { 
                section = section with 
                { 
                    ImageUrl = StorageUrl + section.ImageUrl, 
                    Products = [..section.Products.Select(p => p with { 
                        ImageUrl = p.ImageUrl == null ? null : StorageUrl + p.ImageUrl
                    })]
                };
            }
            return section;
        }
    }
}

//LINQ