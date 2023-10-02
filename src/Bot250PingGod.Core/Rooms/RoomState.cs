using Infrastructure.Seedwork.DataTypes;

namespace Bot250PingGod.Core.Rooms;

public class RoomState : EnumObject
{
    private const string WaitingForPlayersKey = "WFP";

    public static readonly RoomState WaitingForPlayers = new(WaitingForPlayersKey, "Ожидание игроков");

    private RoomState(string key, string name)
        : base(key, name)
    {
    }
}