namespace asp_all.Middleware.Ticks
{
    public static class TicksMiddlewareExtension
    {
        public static IApplicationBuilder UseTicks(this IApplicationBuilder app)
        {
            return app.UseMiddleware<TicksMiddleware>();
        }
    }
}
