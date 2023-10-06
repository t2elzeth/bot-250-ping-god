using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace Bot250PingGod.Application;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public abstract class TelegramMessage
{
    public required long ChatId;
    public required string FullMessageText;

    [UsedImplicitly]
    protected TelegramMessage()
    {
    }
}

public sealed class TelegramCommand : TelegramMessage
{
    public required string Command;

    public string? MessageText;

    public Message Message = null!;

    public bool IsEasterEgg => Command.StartsWith("!");

    private TelegramCommand()
    {
    }

    public static bool TryCreate(long chatId,
                                 string fullMessageText,
                                 Message message,
                                 [NotNullWhen(true)] out TelegramCommand? command)
    {
        command = null;

        if (!fullMessageText.StartsWith("!") && !fullMessageText.StartsWith("/"))
            return false;

        var parsedMessage = fullMessageText.Split(' ');

        var commandText = parsedMessage[0];

        string? messageText = null;
        if (parsedMessage.Length > 1)
            messageText = string.Join(' ', parsedMessage[1..]);

        command = new TelegramCommand
        {
            ChatId          = chatId,
            FullMessageText = fullMessageText,
            Command         = commandText,
            MessageText     = messageText,
            Message         = message
        };

        return true;
    }
}