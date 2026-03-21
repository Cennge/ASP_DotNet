namespace asp_all.Middleware.Auth.Session
{
    public static class AuthSessionExtension
    {
        public static IApplicationBuilder UseAuthSession(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AuthSessionMiddleware>();
        }
    }
}
