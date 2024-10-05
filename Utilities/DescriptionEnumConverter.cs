using Newtonsoft.Json;
using System.ComponentModel;
using System.Reflection;

public class DescriptionEnumConverter<T> : JsonConverter where T : struct, Enum
{
    public override bool CanConvert(Type objectType) => objectType == typeof(T);

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var enumText = reader.Value?.ToString();
        if (enumText == null) return null;

        // Search for enum value based on Description attribute
        foreach (var field in typeof(T).GetFields())
        {
            var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null && descriptionAttribute.Description == enumText)
            {
                return (T)field.GetValue(null);
            }
        }

        // Fallback to enum names if no description matches
        if (Enum.TryParse(enumText, true, out T result))
        {
            return result;
        }

        throw new ArgumentException($"Unknown value: {enumText}");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        // Get the enum's Description attribute (if it exists) for serialization
        var field = typeof(T).GetField(value.ToString());
        var descriptionAttribute = field?.GetCustomAttribute<DescriptionAttribute>();

        writer.WriteValue(descriptionAttribute != null ? descriptionAttribute.Description : value.ToString()); // Default to enum name
    }
}