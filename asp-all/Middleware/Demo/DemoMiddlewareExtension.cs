namespace asp_all.Middleware.Demo
{
    public static class DemoMiddlewareExtension
    {
        public static IApplicationBuilder UseDemo(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DemoMiddleware>();
        }
    }
}
