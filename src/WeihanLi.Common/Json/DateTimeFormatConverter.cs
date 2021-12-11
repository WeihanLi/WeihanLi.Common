using Newtonsoft.Json.Converters;

namespace WeihanLi.Common.Json
{
    public class DateTimeFormatConverter : IsoDateTimeConverter
    {
        public DateTimeFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
