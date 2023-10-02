using System.Reflection;
using Autofac;
using Serilog;

namespace DbSeeds.Core;

public sealed class MigrationSeedRunner
{
    private readonly string _environment;
    private readonly IContainer _container;

    public MigrationSeedRunner(string environment,
                               IContainer container)
    {
        _environment = environment;
        _container   = container;
    }

    public async Task RunAsync<TMigrationSeed>()
        where TMigrationSeed : SeedBundle
    {
        var seedName = typeof(TMigrationSeed).Name;

        var migrationMethods = typeof(TMigrationSeed).GetMethods(BindingFlags.Instance | BindingFlags.Public);

        var migrationSeed = _container.Resolve<TMigrationSeed>();

        foreach (var migrationMethod in migrationMethods)
        {
            var migrationSeedAttribute = migrationMethod.GetCustomAttribute<MigrationSeedAttribute>();
            if (migrationSeedAttribute is null)
                continue;

            if (!migrationSeedAttribute.CanRun(_environment))
                continue;

            Log.Logger.Information("Running {SeedName}.{Method}", seedName, migrationMethod.Name);

            var result = migrationMethod.Invoke(migrationSeed, Array.Empty<object>());

            if (result is Task task)
                await task;

            foreach (var (tag, entity) in migrationSeed.TaggedEntities)
                SeedContext.PutTag(tag, entity.Id);
        }
    }
}