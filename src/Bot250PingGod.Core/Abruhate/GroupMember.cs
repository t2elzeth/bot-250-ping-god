﻿using CSharpFunctionalExtensions;
using Infrastructure.Seedwork.DataTypes;

namespace Bot250PingGod.Core.Abruhate;

public class GroupMember : Entity
{
    public virtual bool IsDeleted { get; protected init; }

    public virtual string Username { get; protected init; } = null!;

    public virtual long AnabruhateCount { get; protected set; }

    public virtual long? ChatId { get; protected set; }

    public virtual UtcDateTime LastAnabruhateDateTime { get; protected set; } = null!;

    public virtual long LastHourAnabruhateCount { get; protected set; }

    protected GroupMember()
    {
    }

    public static GroupMember Create(UtcDateTime dateTime,
                                     string username,
                                     long chatId)
    {
        return new GroupMember
        {
            IsDeleted              = false,
            Username               = username,
            AnabruhateCount        = 0,
            ChatId                 = chatId,
            LastAnabruhateDateTime = dateTime
        };
    }

    public virtual bool CanAnabruhate()
    {
        return LastHourAnabruhateCount < 3;
    }

    public virtual void Anabruhate()
    {
        AnabruhateCount++;
    }

    public virtual void UpdateChatId(long chatId)
    {
        ChatId = chatId;
    }

    public virtual void IncreaseAnabruhateCount()
    {
        LastHourAnabruhateCount++;
    }

    public virtual void UpdateLastAnabruhateDateTime(UtcDateTime dateTime)
    {
        LastAnabruhateDateTime  = dateTime;
        LastHourAnabruhateCount = 0;
    }
}