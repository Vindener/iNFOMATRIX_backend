using System.Text.Json.Serialization;

namespace StarterApp.Infrastructure.Auth;


[JsonConverter(typeof(SupabaseErrorConverter))]
public class SupabaseError
{
    public int Code { get; set; }

    public string ErrorCode { get; set; } = default!;

    public string Message { get; set; } = default!;
}
