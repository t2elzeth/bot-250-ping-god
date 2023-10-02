using System.Text;
using System.Text.Json.Serialization;

namespace Infrastructure.Seedwork.Validation;

public class SystemError
{
    public string Message { get; set; } = null!;

    public IDictionary<string, string>? ParameterErrors { get; private set; }

    public static readonly SystemError InternalServerError = new("Внутренняя ошибка системы");
    
    public static readonly SystemError FillFieldMessage = new("Заполните поле");
    public static readonly SystemError WrongFormat = new("Неверный формат");
    
    public static readonly SystemError AtLeastOnePropertyMustBeDefined = new("Хотя-бы одно поле должно быть заполнено");

    [JsonConstructor]
    public SystemError()
    {
    }

    public SystemError(string message)
    {
        Message = message;
    }

    public SystemError(string message, IDictionary<string, string> parameterErrors)
    {
        Message         = message;
        ParameterErrors = parameterErrors;
    }

    public void SetError(string parameterName, string errorMessage)
    {
        ParameterErrors ??= new Dictionary<string, string>();

        ParameterErrors[parameterName] = errorMessage;
    }

    public static SystemError FillField(string parameterName)
    {
        return new SystemError("Проверьте введенные значения", new Dictionary<string, string>
        {
            [parameterName] = FillFieldMessage.Message
        });
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Сообщение: {Message}");

        if (ParameterErrors is not null)
        {
            sb.Append("; Параметры: {");
            var addComma = false;
            foreach (var (key, value) in ParameterErrors)
            {
                if (addComma)
                    sb.Append(", ");

                sb.Append($"'{key}': '{value}'");

                addComma = true;
            }

            sb.Append('}');

            return sb.ToString();
        }

        return string.Empty;
    }
}