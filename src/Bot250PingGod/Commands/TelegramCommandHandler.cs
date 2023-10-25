using Autofac;
using Bot250PingGod.Core.Groups;
using Bot250PingGod.Groups;
using Infrastructure.DataAccess;
using Infrastructure.Seedwork.Providers;
using Microsoft.Extensions.Logging;
using NHibernate;

namespace Bot250PingGod.Commands;

public interface ITelegramCommandHandler
{
    public Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken);
}

public sealed class TelegramCommandHandler
{
    private readonly ILogger<TelegramCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly GroupRepository _groupRepository;
    private readonly MemberRepository _memberRepository;
    private readonly GroupMemberRepository _groupMemberRepository;
    private readonly ILifetimeScope _lifetimeScope;

    public TelegramCommandHandler(ILogger<TelegramCommandHandler> logger,
                                  IDateTimeProvider dateTimeProvider,
                                  GroupRepository groupRepository,
                                  MemberRepository memberRepository,
                                  GroupMemberRepository groupMemberRepository,
                                  ILifetimeScope lifetimeScope)
    {
        _logger                = logger;
        _dateTimeProvider      = dateTimeProvider;
        _groupRepository       = groupRepository;
        _memberRepository      = memberRepository;
        _groupMemberRepository = groupMemberRepository;
        _lifetimeScope         = lifetimeScope;
    }

    public async Task HandleAsync(TelegramCommand command, CancellationToken cancellationToken)
    {
        var dateTime = _dateTimeProvider.Now();
        var message  = command.Message;

        if (message.From is null)
            return;

        var sessionFactory = _lifetimeScope.Resolve<ISessionFactory>();

        using (_logger.BeginScope("Command#{TelegramCommand}", command.Command))
        await using (DbSession.Bind(sessionFactory))
        {
            await using var dbTransaction = new DbTransaction();


            try
            {
                _logger.LogInformation("Начало обработки команды {TelegramCommand}", command.Command);

                if (!_lifetimeScope.TryResolveKeyed<ITelegramCommandHandler>(command.Command, out var handler))
                {
                    _logger.LogWarning("Невозможно обработать команду {TelegramCommand}, не найден обработчик",
                                       command.Command);
                    return;
                }

                var group = await _groupRepository.TryGetByChatIdAsync(message.Chat.Id, cancellationToken);
                if (group is null)
                {
                    group = Group.Create(title: message.Chat.Title,
                                         chatId: message.Chat.Id,
                                         configuration: GroupConfiguration.CreateDefault());

                    await _groupRepository.SaveAsync(group, cancellationToken);
                }

                var member = await _memberRepository.TryGetByChatIdAsync(message.From.Id, cancellationToken);
                if (member is null)
                {
                    member = Member.Create(username: message.From.Username,
                                           firstName: message.From.FirstName,
                                           lastName: message.From.LastName,
                                           userId: message.From.Id);

                    await _memberRepository.SaveAsync(member, cancellationToken);
                }

                var groupMember = await _groupMemberRepository.TryGetAsync(group.Id, member.Id, cancellationToken);
                if (groupMember is null)
                {
                    groupMember = GroupMember.Create(dateTime: dateTime,
                                                     group: group,
                                                     member: member);

                    await _groupMemberRepository.SaveAsync(groupMember, cancellationToken);
                }

                await handler.HandleAsync(command, cancellationToken);

                await dbTransaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при обработке команды {TelegramCommand}",
                                 command.Command);

                await dbTransaction.RollbackAsync();
            }
        }
    }
}