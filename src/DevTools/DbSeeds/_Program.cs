using System.Reflection;
using Autofac;
using CommandLine;
using DbSeeds;
using DbSeeds.BotSeedBundles;
using DbSeeds.Core;
using DbSeeds.Extensions;
using Infrastructure.DataAccess;
using Infrastructure.Logging;
using Infrastructure.Seedwork.Providers;
using Microsoft.Extensions.Configuration;
using Serilog;

var parserResult = Parser.Default.ParseArguments<Arguments>(args);

var arguments = parserResult.Value;

if (arguments is null)
    return;

var environment = arguments.Environment.ToLowerInvariant();

SeedEnvironmentContext.SetupEnvironment(environment);

var configurationBuilder = new ConfigurationBuilder()
                           .AddJsonFile("appsettings.json")
                           .AddJsonFile($"appsettings.{environment}.json", optional: true);

var configuration = configurationBuilder.Build();

Log.Logger = new LoggerBuilder().WithApplicationName("DbSeeds").Build(configuration);
Log.Logger.Information("Current environment: {Environment}", environment);

ConnectionStringsManager.ReadFromConfiguration(configuration);

var containerBuilder = new ContainerBuilder();

containerBuilder.RegisterAssemblyModules(Assembly.Load("Bot250PingGod.Application"));
containerBuilder.RegisterSeedBundles(Assembly.Load("DbSeeds"));

var systemEnvironment = new SystemEnvironment(TimeZoneInfo.Local, standby: false);
containerBuilder.RegisterInstance(systemEnvironment);

containerBuilder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
var container = containerBuilder.Build();

var seedRunner = new MigrationSeedRunner(environment, container);

await using (DbSession.Bind(NhSessionFactory.Instance))
using (new SeedContext())
{
    await using var transaction = new DbTransaction();

    await seedRunner.RunAsync<GroupMembersSeedBundle>();

    await transaction.CommitAsync();

    Log.CloseAndFlush();
}