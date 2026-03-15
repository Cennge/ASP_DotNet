namespace asp_all.Services.DateTime
{
    public static class DateTimeExtension
    {
        public static IServiceCollection AddDateTime(this IServiceCollection services)
        {
            return services.AddSingleton<IDateTimeService, NationalDateTimeService>();
        }
    }
}
