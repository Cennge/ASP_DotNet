using CenngeShop.Data.Entities;

namespace CenngeShop.Models.Shop.Admin
{
    public class AdminDiscountViewModel
    {
        public List<Discount> Discounts { get; set; } = [];
        public List<ShopProduct> Products { get; set; } = [];
        public List<DiscountDetail> DiscountDetails { get; set; } = [];
    }
}
