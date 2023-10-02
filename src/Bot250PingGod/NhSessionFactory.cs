using Infrastructure.DataAccess;
using NHibernate;

namespace Bot;

public static class NhSessionFactory
{
    public static ISessionFactory Instance { get; }

    static NhSessionFactory()
    {
        Instance = new SessionFactoryBuilder()
                   .AddFluentMappingsFrom("Bot250PingGod.Application")
                   .Build();
    }
}