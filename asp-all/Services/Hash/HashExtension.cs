namespace asp_all.Services.Hash
{
    public static class HashExtension
    {
        public static IServiceCollection AddHash(this IServiceCollection services)
        {
            //return services.AddSingleton<IHashService, Md5HashService>();
            return services.AddSingleton<IHashService, ShaHashService>();
            //return services.AddTransient<IHashService, ShaHashService>();
            //return services.AddScoped<IHashService, ShaHashService>();
        }
    }
}
