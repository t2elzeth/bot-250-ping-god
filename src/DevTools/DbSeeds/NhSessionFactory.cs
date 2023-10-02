using Infrastructure.DataAccess;
using NHibernate;
using Environment = NHibernate.Cfg.Environment;

namespace DbSeeds;

public static class NhSessionFactory
{
    public static ISessionFactory Instance { get; }

    static NhSessionFactory()
    {
        Instance = new SessionFactoryBuilder()
                   .AddFluentMappingsFrom("Bot250PingGod.Application")
                   .ExposeConfiguration(cfg => cfg.SetProperty(Environment.Hbm2ddlKeyWords, "none"))
                   .Build();
    }
}