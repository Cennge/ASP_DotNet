using asp_all.Data.Entities;

namespace asp_all.Models.Shop
{
    public class ShopSectionViewModel
    {
        public ShopSection? ShopSection { get; set; }
        public ShopSection[] ShopSections { get; set; } = [];

        public Cart? ActiveCart { get; set; }
    }
}
