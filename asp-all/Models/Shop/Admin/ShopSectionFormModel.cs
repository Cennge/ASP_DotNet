using System.Text.Json.Serialization;

namespace asp_all.Models.Shop.Admin
{
    public class ShopSectionFormModel
    {
        public String Title { get; set; } = null!;

        public String Description { get; set; } = null!;

        public String Slug { get; set; } = null!;

        [JsonIgnore]
        public IFormFile? ImageFile { get; set; }
        public String? ImageUrl { get; set; } = null!;

        public String Button { get; set; } = null!;
    }
}
