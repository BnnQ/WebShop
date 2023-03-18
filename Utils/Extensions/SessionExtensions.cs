using System.Text.Json;
using System.Text.Json.Serialization;

namespace Homework.Utils.Extensions;

public static class SessionExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.Preserve
    };
    
    /// <exception cref="ArgumentNullException">If passed argument <paramref name="key"/> or <paramref name="value"/> is null</exception>
    public static void SetValue<TValue>(this ISession session, string key, TValue value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var serializedValue = JsonSerializer.Serialize(value, SerializerOptions);
        session.SetString(key, serializedValue);
    }

    /// <exception cref="ArgumentNullException">If passed argument <paramref name="key"/> is null</exception>
    public static TValue? GetValueOrDefault<TValue>(this ISession session, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        
        var serializedValue = session.GetString(key);
        return !string.IsNullOrWhiteSpace(serializedValue)
            ? JsonSerializer.Deserialize<TValue>(serializedValue, SerializerOptions)
            : default(TValue);
    }
}