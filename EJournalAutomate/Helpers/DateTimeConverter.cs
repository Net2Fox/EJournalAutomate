using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EJournalAutomate.Helpers
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateString = reader.GetString();
            if (string.IsNullOrEmpty(dateString))
            {
                return DateTime.MinValue;
            }

            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return DateTime.Parse(dateString, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
        }
    }
}
