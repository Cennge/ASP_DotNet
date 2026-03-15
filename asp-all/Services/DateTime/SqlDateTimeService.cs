namespace asp_all.Services.DateTime
{
    public class SqlDateTimeService : IDateTimeService
    {
        public string GetDate(System.DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }

        public string GetTime(System.DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss.fff");
        }
    }
}
