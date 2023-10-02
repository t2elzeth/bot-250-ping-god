using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure.AspNetCore.Nh;
using Infrastructure.AspNetCore.Serialization;
using Infrastructure.AspNetCore.Timeout;
using Infrastructure.DataAccess;
using Infrastructure.Logging;
using Infrastructure.Seedwork.Extensions;
using Infrastructure.Seedwork.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;

namespace Infrastructure.AspNetCore;

public class WebApplicationBoostrap
{
    private bool _addXmlOutputFormatter;
    private readonly WebApplicationBuilder _builder;
    private Action<IConfiguration, ContainerBuilder>? _containerBuilder;
    private Action<WebApplicationBuilder>? _build;
    private Action<IMvcBuilder>? _buildMvc;
    private Action<WebApplication>? _configure;

    private Action<JsonOptions>? _configureJsonSerializer;

    private Action<CorsOptions>? _configureCors;

    private WebApplicationBoostrap(WebApplicationBuilder builder)
    {
        _builder = builder;
    }

    public static WebApplicationBoostrap Create(string[] args)
    {
        var webApplicationOptions = new WebApplicationOptions
        {
            Args            = args,
            ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
        };

        var builder = WebApplication.CreateBuilder(webApplicationOptions);

        builder.Configuration.AddJsonFile("appsettings/appsettings.production.json", optional: true, reloadOnChange: true);

        return new WebApplicationBoostrap(builder);
    }

    public WebApplicationBoostrap ContainerBuilder(Action<IConfiguration, ContainerBuilder> containerBuilder)
    {
        _containerBuilder = containerBuilder;
        return this;
    }

    public WebApplicationBoostrap Build(Action<WebApplicationBuilder> build)
    {
        _build = build;
        return this;
    }

    public WebApplicationBoostrap BuildMvc(Action<IMvcBuilder> buildMvc)
    {
        _buildMvc = buildMvc;
        return this;
    }

    public WebApplicationBoostrap Configure(Action<WebApplication> configure)
    {
        _configure = configure;
        return this;
    }

    public WebApplicationBoostrap ConfigureJsonSerializer(Action<JsonOptions> configureJsonSerializer)
    {
        _configureJsonSerializer = configureJsonSerializer;
        return this;
    }

    public WebApplicationBoostrap AddXmlOutputFormatter()
    {
        _addXmlOutputFormatter = true;
        return this;
    }

    public WebApplicationBoostrap ConfigureCors(Action<CorsOptions> configureCors)
    {
        _configureCors = configureCors;
        return this;
    }

    public void Start(string applicationName)
    {
        var configuration = _builder.Configuration;

        Log.Logger = new LoggerBuilder()
                     .WithApplicationName(applicationName)
                     .Build(configuration);

        var logger = Log.Logger.ForContext<WebApplicationBoostrap>();

        try
        {
            logger.Information("{ApplicationName} is starting", applicationName);

            _builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(containerBuilder => //
            {
                _containerBuilder?.Invoke(configuration, containerBuilder);
            }));

            _builder.Host
                    .UseWindowsService()
                    .UseSerilog();

            ConnectionStringsManager.ReadFromConfiguration(configuration);

            _build?.Invoke(_builder);

            _builder.Services.AddLogging(configure => { configure.AddSerilog(); });

            //Нужно для AppMetrics
            _builder.Services.Configure<KestrelServerOptions>(options => { options.AllowSynchronousIO = true; });

            if (_configureCors is not null)
            {
                _builder.Services.AddCors(c => //
                {
                    _configureCors.Invoke(c);
                });
            }

            var mvcBuilder = _builder.Services
                                     .AddMvc(mvcOptions =>
                                     {
                                         if (_addXmlOutputFormatter)
                                         {
                                             mvcOptions.OutputFormatters.RemoveType<XmlSerializerOutputFormatter>();
                                             mvcOptions.OutputFormatters.Add(new XmlSerializerOutputFormatterNamespace());
                                         }

                                         mvcOptions.Filters.Add<DbSessionAttributeActionFilter>();
                                     })
                                     .AddJsonOptions(options => //
                                     {
                                         if (_configureJsonSerializer is null)
                                         {
                                             options.JsonSerializerOptions.ConfigureSystem();
                                         }
                                         else
                                         {
                                             _configureJsonSerializer?.Invoke(options);
                                         }
                                     });

            if (_addXmlOutputFormatter)
                mvcBuilder.AddXmlSerializerFormatters();

            _builder.Services.Configure<MvcOptions>(options =>
            {
                options.ModelBinderProviders.RemoveType<CancellationTokenModelBinderProvider>();
                options.ModelBinderProviders.Insert(0, new TimeoutCancellationTokenModelBinderProvider());
            });

            _builder.Services.Configure<TimeoutOptions>(_ => { });

            var metrics = AppMetrics.CreateDefaultBuilder()
                                    .Configuration.Configure(options => //
                                    {
                                        options.DefaultContextLabel = "emps";
                                    })
                                    .Build();
            _builder.Services.AddMetrics(metrics);
            _builder.Services.AddMetricsEndpoints();

            _buildMvc?.Invoke(mvcBuilder);

            _builder.Services.AddEndpointsApiExplorer();
            _builder.Services.AddSwaggerGen();

            _builder.Services.AddHealthChecks();

            var app = _builder.Build();

            app.UseMetricsEndpoint(new MetricsPrometheusTextOutputFormatter());

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapHealthChecks("/healthz")
               .AllowAnonymous();

            _configure?.Invoke(app);

            var systemEnvironment = app.Services.GetRequiredService<SystemEnvironment>();
            logger.Information("SystemEnvironment: {@SystemEnvironment}", systemEnvironment);

            app.Run();

            Log.CloseAndFlush();
        }
        catch (Exception ex)
        {
            logger.Fatal(ex, "Host terminated unexpectedly");
            Log.CloseAndFlush();
            Environment.Exit(-1);
        }
    }
}