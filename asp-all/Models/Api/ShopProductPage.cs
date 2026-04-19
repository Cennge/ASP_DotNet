namespace asp_all.Models.Api
{
    public class ShopProductPage
    {
        public ShopProductModel Product { get; set; } = null!;
        public IEnumerable<ShopProductModel> Recommended { get; set; } = [];
    }
}
