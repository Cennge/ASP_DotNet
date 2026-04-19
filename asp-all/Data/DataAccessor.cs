using asp_all.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace asp_all.Data
{
    public class DataAccessor(DataContext dataContext)
    {
        private readonly DataContext _dataContext = dataContext;

        public UserAccess? GetUserAccessByLogin(String login) =>
            _dataContext.UserAccesses.FirstOrDefault(a => a.Login == login);

        public Cart GetOrCreateActiveCart(Guid userId)
        {
            Cart? cart = this.GetActiveCart(userId);

            if (cart == null)
            {
                cart = new Cart()
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreateDt = DateTime.Now
                };
                _dataContext.Carts.Add(cart);
            }
            return cart;
        }

        public Cart? GetActiveCart(Guid userId)
        {
            return _dataContext
                .Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefault(c =>
                c.UserId == userId && c.DeleteDt == null && c.OrderDt == null);
        }

        public IEnumerable<ShopSection> AllShopSections()
        {
            return _dataContext
                .ShopSections
                .AsNoTracking()
                .Where(s => s.DeletedAt == null)
                .OrderBy(s => s.OrderInPrice)
                .AsEnumerable();
        }

        public ShopSection? GetShopSectionBySlug(String slug)
        {
            return _dataContext
                .ShopSections
                .Include(s => s.Products)
                .AsNoTracking()
                .FirstOrDefault(s => s.Slug == slug && s.DeletedAt == null);
        }

        public ShopProduct? GetShopProductBySlug(String slugOrId)
        {
            return _dataContext
                .ShopProducts
                .AsNoTracking()
                .FirstOrDefault(s => (s.Slug == slugOrId || s.Id.ToString() == slugOrId) && s.DeletedAt == null);
        }
    }
}
