using Bot250PingGod.Core.Abruhate;
using DbSeeds.Core;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace DbSeeds.BotSeedBundles;

public class AnabruhatedUsersSeedBundle : SeedBundle
{
    [MigrationSeed, UsedImplicitly]
    public async Task Admin()
    {
        await AnabruhatedUser.Create(username: "t2elzeth")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "Qwetryq")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "Konsteynto")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "snakerukapup")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "a1ibekk")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "abu_hurayr4")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "Ai8oss")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "Teng1Zz")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "arigatonahui")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "kassellnotxd")
                             .SaveAsync();

        await AnabruhatedUser.Create(username: "bektur_ae")
                             .SaveAsync();
    }
}