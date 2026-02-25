using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace fullcalendarcore.Library
{
    /// <summary>
    /// JSON converter that handles enum serialization/deserialization as numbers
    /// This ensures JavaScript integers are properly bound to C# enums
    /// </summary>
    public class JsonNumberEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Handle both number and string inputs
            if (reader.TokenType == JsonTokenType.Number)
            {
                var enumValue = reader.GetInt32();
                return (T)Enum.ToObject(typeof(T), enumValue);
            }
            else if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (Enum.TryParse<T>(stringValue, true, out var result))
                {
                    return result;
                }
            }

            // Default to first enum value if parsing fails
            return default(T);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // Write as integer
            writer.WriteNumberValue(Convert.ToInt32(value));
        }
    }
}
