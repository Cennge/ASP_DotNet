using asp_all.Data.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace asp_all.Models.Shop.Admin
{
    public class ShopAdminViewModel
    {
        public ShopSectionFormModel? ShopSectionFormModel { get; set; }

        public ModelStateDictionary? ShopSectionModelState { get; set; }


        public ShopProductFormModel? ShopProductFormModel { get; set; }

        public ModelStateDictionary? ShopProductModelState { get; set; }


        public List<ShopSection> ShopSections { get; set; } = [];
    }
}
