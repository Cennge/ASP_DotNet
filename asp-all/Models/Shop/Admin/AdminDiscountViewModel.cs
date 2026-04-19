using asp_all.Data.Entities;

namespace asp_all.Models.Shop.Admin
{
    public class AdminDiscountViewModel
    {
        public List<Discount> Discounts { get; set; } = [];
        public List<ShopProduct> Products { get; set; } = [];
        public List<DiscountDetail> DiscountDetails { get; set; } = [];
    }
}
