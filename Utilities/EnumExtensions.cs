using System.ComponentModel;

public static class EnumExtensions
{
    public static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct, Enum
    {
        var field = typeof(TEnum).GetField(enumValue.ToString());
        var attribute = field?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() as DescriptionAttribute;

        return attribute?.Description ?? enumValue.ToString();
    }
}