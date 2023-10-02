using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Infrastructure.AspNetCore.Nh;

internal static class ActionDescriptorExtensions
{
    private static ControllerActionDescriptor AsControllerActionDescriptor(this ActionDescriptor actionDescriptor)
    {
        if (!actionDescriptor.IsControllerAction())
        {
            throw new InvalidOperationException($"{nameof(actionDescriptor)} should be type of {nameof(ControllerActionDescriptor)}");
        }

        return (ControllerActionDescriptor)actionDescriptor;
    }

    public static MethodInfo GetMethodInfo(this ActionDescriptor actionDescriptor)
    {
        return actionDescriptor.AsControllerActionDescriptor().MethodInfo;
    }

    public static bool IsControllerAction(this ActionDescriptor actionDescriptor)
    {
        return actionDescriptor is ControllerActionDescriptor;
    }
}