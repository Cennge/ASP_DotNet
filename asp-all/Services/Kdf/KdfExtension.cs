namespace asp_all.Services.Kdf
{
    public static class KdfExtension
    {
        public static IServiceCollection AddKdf(this IServiceCollection services)
        {
            return services.AddSingleton<IKdfService, PbKdfService>();
        }
    }
}
