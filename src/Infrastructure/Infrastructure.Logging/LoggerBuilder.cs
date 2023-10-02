using Infrastructure.Seedwork.DataTypes;
using Infrastructure.Seedwork.Validation;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Exceptions;

namespace Infrastructure.Logging;

public sealed class LoggerBuilder
{
    private string? _applicationName;

    public LoggerBuilder WithApplicationName(string applicationName)
    {
        _applicationName = applicationName;
        return this;
    }
    
    public ILogger Build(IConfiguration configuration)
    {
        if (_applicationName is null)
            throw new InvalidOperationException("Application name is not defined");
        
        var loggerConfiguration = new LoggerConfiguration()
                                  .Destructure.ByTransforming<TimeZoneInfo>(d => d.Id)
                                  .Destructure.ByTransforming<Date>(d => d.Value)
                                  .Destructure.ByTransforming<UtcDateTime>(d => d.Value)
                                  .Destructure.ByTransformingWhere<EnumObject>(x => typeof(EnumObject).IsAssignableFrom(x), o => o.Name)
                                  .Destructure.ByTransforming<SystemError>(d => new
                                  {
                                      d.Message,
                                      d.ParameterErrors
                                  })
                                  .Enrich.WithProperty("Application", _applicationName)
                                  .Enrich.WithExceptionDetails()
                                  .Enrich.WithMachineName()
                                  .Enrich.FromLogContext()
                                  .ReadFrom.Configuration(configuration);

        return loggerConfiguration.CreateLogger();
    }
}