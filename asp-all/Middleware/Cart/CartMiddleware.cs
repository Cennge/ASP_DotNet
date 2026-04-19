using asp_all.Data;
using asp_all.Data.Entities;
using System.Security.Claims;

namespace asp_all.Middleware.Cart
{
    public class CartMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, DataAccessor dataAccessor)
        {
            if (context.User.Identity?.IsAuthenticated ?? false)
            {
                String userLogin = context.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
                UserAccess? userAccess = dataAccessor.GetUserAccessByLogin(userLogin);
                if (userAccess != null)
                {
                    context.Items["ActiveCart"] = dataAccessor.GetActiveCart(userAccess.UserId);
                }
            }
            await _next(context);
        }

    }
}
