using Infrastructure.DataAccess;
using NHibernate;

namespace Bot250PingGod;

public static class NhSessionFactory
{
    public static ISessionFactory Instance { get; }

    static NhSessionFactory()
    {
        Instance = new SessionFactoryBuilder()
                   .AddFluentMappingsFrom("Bot250PingGod")
                   .Build();
    }
}