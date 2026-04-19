using asp_all.Data.Entities;

namespace asp_all.Models.Shop
{
    public class ShopProductViewModel
    {
        public ShopProduct? ShopProduct { get; set; }
        public ShopProduct[] PromoProducts { get; set; } = [];
        public ShopSection[] ShopSections { get; set; } = [];
    }
}
