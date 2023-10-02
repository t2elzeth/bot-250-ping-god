using Bot250PingGod.Core.Abruhate;
using DbSeeds.Core;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace DbSeeds.BotSeedBundles;

public class GroupMembersSeedBundle : SeedBundle
{
    [MigrationSeed, UsedImplicitly]
    public async Task Admin()
    {
        await GroupMember.Create(username: "t2elzeth")
                         .SaveAsync();

        await GroupMember.Create(username: "Qwetryq")
                         .SaveAsync();

        await GroupMember.Create(username: "Konsteynto")
                         .SaveAsync();

        await GroupMember.Create(username: "snakerukapup")
                         .SaveAsync();

        await GroupMember.Create(username: "a1ibekk")
                         .SaveAsync();

        await GroupMember.Create(username: "abu_hurayr4")
                         .SaveAsync();

        await GroupMember.Create(username: "Ai8oss")
                         .SaveAsync();

        await GroupMember.Create(username: "Teng1Zz")
                         .SaveAsync();

        await GroupMember.Create(username: "arigatonahui")
                         .SaveAsync();

        await GroupMember.Create(username: "kassellnotxd")
                         .SaveAsync();

        await GroupMember.Create(username: "bektur_ae")
                         .SaveAsync();
    }
}