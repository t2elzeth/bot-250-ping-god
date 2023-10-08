using DbSeeds.Core;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace DbSeeds.BotSeedBundles;

public class GroupMembersSeedBundle : SeedBundle
{
    [MigrationSeed, UsedImplicitly]
    public void Admin()
    {
    }
}