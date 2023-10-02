using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.AspNetCore.Extensions;

public static class ConfigurationExtensions
{
    public static void RegisterConfiguration<TConfig>(this WebApplicationBuilder builder, string section) where TConfig : class
    {
        var configuration = builder.Configuration;
        var services      = builder.Services;

        services.AddSingleton(configuration.Get<TConfig>(section));
    }

    private static TConfig Get<TConfig>(this IConfiguration configuration, string section)
    {
        return configuration.GetSection(section).Get<TConfig>()!;
    }
}
