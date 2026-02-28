using System.Text.Json;
using System.Text.Json.Serialization;

namespace StarterApp.Infrastructure.Auth;

public class SupabaseErrorConverter : JsonConverter<SupabaseError>
{
    public override SupabaseError Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        int code = root.TryGetProperty("code", out var codeProp) && codeProp.TryGetInt32(out var c) ? c : 0;

        string GetString(params string[] keys)
        {
            foreach (var key in keys)
            {
                if (root.TryGetProperty(key, out var prop) && !string.IsNullOrEmpty(prop.GetString()))
                    return prop.GetString()!;
            }
            return string.Empty;
        }

        return new SupabaseError
        {
            Code = code,
            ErrorCode = GetString("error_code", "error"),
            Message = GetString("msg", "error_description")
        };
    }

    public override void Write(
        Utf8JsonWriter writer,
        SupabaseError value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("code", value.Code);
        writer.WriteString("error_code", value.ErrorCode);
        writer.WriteString("msg", value.Message);
        writer.WriteEndObject();
    }
}