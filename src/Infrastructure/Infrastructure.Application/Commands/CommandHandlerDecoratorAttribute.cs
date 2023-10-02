namespace Infrastructure.Application.Commands;

[AttributeUsage(AttributeTargets.Class)]
public class CommandHandlerDecoratorAttribute : Attribute
{
    public Type DecoratorType { get; }

    public CommandHandlerDecoratorAttribute(Type decoratorType)
    {
        DecoratorType = decoratorType;
    }
}