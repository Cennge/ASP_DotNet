using CenngeShop.Services.Hash;

namespace CenngeShop.Middleware.Ticks
{
    public class TicksMiddleware
    {
        private readonly RequestDelegate _next;

        public TicksMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Items.Add("MiddlewareTicks", DateTime.Now.Ticks);

            await _next(context);
        }
    }
}
