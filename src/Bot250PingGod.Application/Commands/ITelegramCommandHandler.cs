namespace Bot250PingGod.Application.Commands;

public interface ITelegramCommandHandler
{
    public Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken);
}