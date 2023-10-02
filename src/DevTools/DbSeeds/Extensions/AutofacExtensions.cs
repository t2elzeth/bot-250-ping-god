using System.Reflection;
using Autofac;
using DbSeeds.Core;

namespace DbSeeds.Extensions;

public static class AutofacExtensions
{
    public static void RegisterSeedBundles(this ContainerBuilder cb, Assembly assembly)
    {
        var seedBundleTypes = assembly.GetTypes()
                                      .Where(t => t.IsAssignableTo<SeedBundle>())
                                      .Where(t => !t.IsAbstract);

        foreach (var seedBundleType in seedBundleTypes)
        {
            cb.RegisterType(seedBundleType).SingleInstance();
        }
    }
}