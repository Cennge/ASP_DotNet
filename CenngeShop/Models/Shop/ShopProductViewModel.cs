using CenngeShop.Data.Entities;

namespace CenngeShop.Models.Shop
{
    public class ShopProductViewModel
    {
        public ShopProduct? ShopProduct { get; set; }
        public ShopProduct[] PromoProducts { get; set; } = [];
        public ShopSection[] ShopSections { get; set; } = [];
    }
}
