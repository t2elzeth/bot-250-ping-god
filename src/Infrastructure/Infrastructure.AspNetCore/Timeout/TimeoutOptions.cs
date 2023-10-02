namespace Infrastructure.AspNetCore.Timeout;

public sealed class TimeoutOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}