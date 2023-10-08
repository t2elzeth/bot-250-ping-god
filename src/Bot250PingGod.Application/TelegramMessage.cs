using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace Bot250PingGod.Application;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class TelegramMessage
{
    public required Message Message;

    [UsedImplicitly]
    protected TelegramMessage()
    {
    }
}

public sealed class TelegramCommand : TelegramMessage
{
    public required string Command;

    public bool IsEasterEgg => Command.StartsWith("!");

    private TelegramCommand()
    {
    }

    public static bool TryCreate(Message message,
                                 [NotNullWhen(true)] out TelegramCommand? command)
    {
        command = null;

        var fullMessageText = message.Text!;

        if (!fullMessageText.StartsWith("!") && !fullMessageText.StartsWith("/"))
            return false;

        var parsedMessage = fullMessageText.Split(' ');

        var commandText = parsedMessage[0];

        command = new TelegramCommand
        {
            Command = commandText,
            Message = message
        };

        return true;
    }
}