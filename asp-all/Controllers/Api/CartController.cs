using asp_all.Data;
using asp_all.Data.Entities;
using asp_all.Middleware.Auth.Token;
using asp_all.Models.Api;
using asp_all.Models.Api.Cart;
using asp_all.Services.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace asp_all.Controllers.Api
{
    [Route("api/cart")]
    [ApiController]
    public class CartController(DataContext dataContext, IStorageService storageService, DataAccessor dataAccessor) : ControllerBase
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IStorageService _storageService = storageService;
        private readonly DataAccessor _dataAccessor = dataAccessor;
        private RestResponse restResponse = new();

        private UserAccess? CheckAuth()
        {
            if (!(HttpContext.User.Identity?.IsAuthenticated ?? false))
            {
                restResponse.Data = HttpContext.Items[nameof(AuthTokenMiddleware)]?.ToString() ?? string.Empty;
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return null;
            }
            String userLogin = HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            UserAccess? userAccess = _dataContext.UserAccesses.FirstOrDefault(a => a.Login == userLogin);
            if (userAccess == null)
            {
                Response.StatusCode = StatusCodes.Status403Forbidden;
                restResponse.Data = "'Sub' not found";
                return null;
            }
            return userAccess;
        }

        [HttpDelete("{id}")]
        public RestResponse UpdateCartItem([FromRoute] String id)
        {
            if (CheckAuth() is UserAccess userAccess)
            {
                try
                {
                    Guid cartItemId;
                    try { cartItemId = Guid.Parse(id); }
                    catch { throw new Exception("cartItemId must be valid UUID"); }
                    // перевіряємо, що даний cartItemId належить авторизованому користувачу
                    Cart cart = _dataAccessor.GetActiveCart(userAccess.UserId)
                        ?? throw new Exception("User has no active cart");
                    CartItem cartItem = cart.CartItems.FirstOrDefault(c => c.Id == cartItemId)
                        ?? throw new Exception("cartItemId belongs no to authorized user");
                    cart.CartItems.Remove(cartItem);
                    CalcCartPrice(cart);
                    restResponse.Data = cart;
                }
                catch (Exception ex)
                {
                    restResponse.Data = ex.Message;
                }
            }
            return restResponse;
        }


        [HttpPut("{id}")]
        public RestResponse UpdateCartItem([FromRoute] String id, int inc)
        {
            if (CheckAuth() is UserAccess userAccess)
            {
                try
                {
                    if (inc == 0)
                    {
                        throw new Exception("Parameter 'inc' could not be empty");
                    }
                    Guid cartItemId;
                    try { cartItemId = Guid.Parse(id); }
                    catch { throw new Exception("cartItemId must be valid UUID"); }
                    Cart cart = _dataAccessor.GetActiveCart(userAccess.UserId)
                        ?? throw new Exception("User has no active cart");
                    CartItem cartItem = cart.CartItems.FirstOrDefault(c => c.Id == cartItemId)
                        ?? throw new Exception("cartItemId belongs no to authorized user");
                    // перевіряємо застосовність інкременту: підсумкова кількість
                    // замовлення не повинна бути 0 чи менша (видалення - окрема точка)
                    // а також не перевищувати наявну кількість товару (Stock)
                    int newQuantity = cartItem.Quantity + inc;
                    if (newQuantity < 0)
                    {
                        throw new Exception("Update fails: negative result obtains");
                    }
                    if (newQuantity == 0)
                    {
                        throw new Exception("Update fails: zero result obtains - Delete is separate endpoint");
                    }
                    if (newQuantity > cartItem.Product.Stock)
                    {
                        throw new Exception($"Update fails: stock limit is {cartItem.Product.Stock}");
                    }
                    cartItem.Quantity = newQuantity;
                    CalcCartPrice(cart);
                    restResponse.Data = cart;
                }
                catch (Exception ex)
                {
                    restResponse.Data = ex.Message;
                }
            }
            return restResponse;
        }

        [HttpPost("{id}")]
        public RestResponse AddProductToCart([FromRoute] String id)
        {
            if (CheckAuth() is UserAccess userAccess)
            {
                Guid productId;
                try {productId = Guid.Parse(id);}
                catch { restResponse.Data = "productId must be valid UUID"; return restResponse; }
                Cart? cart = _dataAccessor.GetOrCreateActiveCart(userAccess.UserId);
                CartItem? cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem == null)
                {
                    cartItem = new()
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = 1
                    };
                    //cart.CartItems.Add(cartItem);
                    _dataContext.CartItems.Add(cartItem);
                    _dataContext.SaveChanges();
                }
                else
                {
                    cartItem.Quantity += 1;
                }
                CalcCartPrice(cart);
                restResponse.Data = cart;
            }
            return restResponse;
        }

        private void CalcCartPrice(Cart cart)
        {
            decimal price = 0;
            foreach (var item in cart.CartItems)
            {
                if (item.DiscountId == null)
                {
                    ShopProduct product = item.Product ?? 
                        _dataAccessor.GetShopProductBySlug(item.ProductId.ToString())!;

                    item.Price = product.Price * item.Quantity;
                }
                else
                {
                    throw new NotImplementedException("CalcCartPrice: product discount");
                }
                price += item.Price;
            }
            if (cart.DiscountId == null)
            {
                cart.Price = price;
            }
            else
            {
                throw new NotImplementedException("CalcCartPrice: cart discount");
            }
            _dataContext.SaveChanges();
        }

        [HttpGet("{id}")]
        public RestResponse LoadOrderDetails([FromRoute] String id)
        {
            if (CheckAuth() is UserAccess userAccess) //pattern matching
            {
                Guid сartId;

                try
                {
                    сartId = Guid.Parse(id);
                }
                catch
                {
                    restResponse.Data = "Parameter 'id' must be a valid UUID";
                    return restResponse;
                }

                var cart = _dataContext
                .Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .AsNoTracking()
                .FirstOrDefault(c => c.Id == сartId);
                if (cart != null)
                {
                    cart = cart with
                    {
                        CartItems = [..cart.CartItems.Select(ci => ci with
                        {
                            Product = ci.Product with
                            {
                                ImageUrl = _storageService.GetPathPrefix() +
                                (ci.Product.ImageUrl ?? "no_image.webp")
                            }
                        })]
                    };
                } 
                else
                {
                    restResponse.Data = "Error 404: Order not found";
                    return restResponse;
                }
                restResponse.Data = cart;
            }
            return restResponse;
        }

        [HttpGet]
        public RestResponse LoadHistory()
        {
            UserAccess? userAccess = CheckAuth();
            if (userAccess == null) return restResponse;


            var history = _dataContext
                .Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .Where(c => c.UserId == userAccess.UserId)
                .OrderByDescending(c => c.CreateDt)
                .AsNoTracking()
                .ToList();

            restResponse.Meta.TotalCount = history.Count;

            restResponse.Data = history;
            return restResponse;
        }

        [HttpPost]
        public RestResponse CreateOrder([FromBody] CartFormModel formModel)
        {
            UserAccess? userAccess = CheckAuth();
            if (userAccess == null) return restResponse;
            Cart? cart = _dataAccessor.GetOrCreateActiveCart(userAccess.UserId);

            List<ShopProduct> productsToUpdate = new();

            foreach (var cartItem in formModel.CartItems)
            {
                ShopProduct product = _dataContext.ShopProducts.FirstOrDefault(p => p.Id == Guid.Parse(cartItem.ProductId))!;

                if (product.Stock < cartItem.Cnt)
                {
                    restResponse.Data = $"Недостатньо на складі товару '{product.Title}'. Замовлено: {cartItem.Cnt}, в наявності: {product.Stock}";
                    return restResponse;
                }
                productsToUpdate.Add(product);
            }

            for (int i = 0; i < formModel.CartItems.Length; i++)
            {
                var cartItem = formModel.CartItems[i];
                var product = productsToUpdate[i];
                product.Stock -= cartItem.Cnt;

                _dataContext.CartItems.Add(new()
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = product.Id,
                    Quantity = cartItem.Cnt,
                    Price = (decimal)cartItem.Price
                });
            }
            cart.Price = (decimal)formModel.Price;
            cart.OrderDt = DateTime.Now;
            _dataContext.SaveChanges();
            restResponse.Data = "Created";
            return restResponse;
        }
    }
}
