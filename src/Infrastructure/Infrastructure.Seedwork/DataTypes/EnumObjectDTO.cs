using JetBrains.Annotations;

namespace Infrastructure.Seedwork.DataTypes;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class EnumObjectDTO
{
    public string Value { get; init; } = null!;

    public string Label { get; init; } = null!;

    public static EnumObjectDTO Create(EnumObject enumObject)
    {
        return new EnumObjectDTO
        {
            Value = enumObject.Key,
            Label = enumObject.Name
        };
    }
}