namespace asp_all.Services.DateTime
{
    public class NationalDateTimeService : IDateTimeService
    {

        public string GetDate(System.DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }

        public string GetTime(System.DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }
    }
}
