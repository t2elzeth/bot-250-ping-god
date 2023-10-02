using Infrastructure.DataAccess;
using Microsoft.AspNetCore.Mvc.Filters;
using NHibernate;

namespace Infrastructure.AspNetCore.Nh;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DbSessionAttribute : Attribute
{
}

public sealed class DbSessionAttributeActionFilter : IAsyncActionFilter
{
    private readonly ISessionFactory _sessionFactory;

    public DbSessionAttributeActionFilter(ISessionFactory sessionFactory)
    {
        _sessionFactory = sessionFactory;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context,
                                             ActionExecutionDelegate next)
    {
        if (!context.ActionDescriptor.IsControllerAction())
        {
            await next();
            return;
        }

        var attribute = AttributeProvider<DbSessionAttribute>.FirstOrDefault(context.ActionDescriptor.GetMethodInfo());

        if (attribute == null)
        {
            await next();
            return;
        }

        using (DbSession.Bind(_sessionFactory))
        {
            await next();
        }
    }
}