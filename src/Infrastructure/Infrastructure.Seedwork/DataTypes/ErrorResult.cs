using System.Text.Json.Serialization;
using Infrastructure.Seedwork.Validation;

namespace Infrastructure.Seedwork.DataTypes;

public class ErrorResult
{
    public long? Code { get; set; }

    public string? Message { get; set; }

    public Dictionary<string, string>? ParameterErrors { get; private set; }

    public ErrorResult()
    {
    }

    [JsonConstructor]
    public ErrorResult(long? code,
                       string? message,
                       Dictionary<string, string>? parameterErrors)
    {
        Code            = code;
        Message         = message;
        ParameterErrors = parameterErrors;
    }

    public static implicit operator ErrorResult(SystemError error)
    {
        var result = new ErrorResult
        {
            Message         = error.Message
        };

        if (error.ParameterErrors is not null)
            result.ParameterErrors = new Dictionary<string, string>(error.ParameterErrors);
        
        return result;
    }
}