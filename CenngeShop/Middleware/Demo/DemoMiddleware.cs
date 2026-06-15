using CenngeShop.Services.Hash;
using CenngeShop.Services.Scoped;
using CenngeShop.Services.Transient;

namespace CenngeShop.Middleware.Demo
{
    public class DemoMiddleware
    {
        private readonly RequestDelegate _next;

        public DemoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IHashService hashService, ScopedService scopedService, TransientService transientService)
        {
            context.Items.Add("FromDemoMiddleware", hashService.GetHashCode());
            context.Items.Add("MiddlewareScopedHash", scopedService.GetHashCode());
            context.Items.Add("MiddlewareTransientHash", transientService.GetHashCode());
            await _next(context);
        }
    }
}
