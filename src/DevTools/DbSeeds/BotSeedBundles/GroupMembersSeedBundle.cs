using Bot250PingGod.Core.Abruhate;
using DbSeeds.Core;
using Infrastructure.Seedwork.Providers;
using JetBrains.Annotations;

// ReSharper disable InconsistentNaming

namespace DbSeeds.BotSeedBundles;

public class GroupMembersSeedBundle : SeedBundle
{
    private readonly IDateTimeProvider _dateTimeProvider;

    public GroupMembersSeedBundle(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    [MigrationSeed, UsedImplicitly]
    public async Task Admin()
    {
        var dateTime = _dateTimeProvider.Now();

        await GroupMember.Create(dateTime: dateTime,
                                 username: "t2elzeth",
                                 chatId: 399344900)
                         .SaveAsync();

        await GroupMember.Create(dateTime: dateTime,
                                 username: "snakerukapup",
                                 chatId: 1064096284)
                         .SaveAsync();

        await GroupMember.Create(dateTime: dateTime,
                                 username: "a1ibekk",
                                 chatId: 639562476)
                         .SaveAsync();

        await GroupMember.Create(dateTime: dateTime,
                                 username: "bektur_ae",
                                 chatId: 813010773)
                         .SaveAsync();
    }
}