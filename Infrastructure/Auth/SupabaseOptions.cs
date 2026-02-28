namespace StarterApp.Infrastructure.Auth;

public sealed class SupabaseOptions
{
    public string Url { get; init; } = string.Empty;

    public string Key { get; init; } = string.Empty;

    public string AdminKey { get; init; } = string.Empty;
}
