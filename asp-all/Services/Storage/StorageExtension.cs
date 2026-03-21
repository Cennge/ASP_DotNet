namespace asp_all.Services.Storage
{
    public static class StorageExtension
    {
        public static IServiceCollection AddStorage(this IServiceCollection services)
        {
            return services.AddSingleton<IStorageService, LocalStorageService>();
        }
    }
}
