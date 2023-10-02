using System.Diagnostics.CodeAnalysis;

namespace Bot250PingGod.Application;

public abstract class TelegramMessage
{
    public required long ChatId;
    public required string FullMessageText;

    protected TelegramMessage()
    {
    }
}

public sealed class TelegramCommand : TelegramMessage
{
    public const string AskCommand = "/ask";
    public const string AbruhateCommand = "/abruhate";

    public required string Command;

    public string? MessageText;

    public bool IsEasterEgg => Command.StartsWith("!");

    private TelegramCommand()
    {
    }

    public static bool TryCreate(long chatId, string fullMessageText, [NotNullWhen(true)] out TelegramCommand? command)
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
            MessageText     = messageText
        };

        return true;
    }
}