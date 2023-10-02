namespace Infrastructure.Seedwork.DataTypes;

public abstract class EnumObject
{
    public string Key { get; }

    public string Name { get; }

    protected EnumObject(string key, string name)
    {
        Key  = key;
        Name = name;
    }

    public override string ToString()
    {
        return $"{Key} ({Name})";
    }

    private bool Equals(EnumObject other)
    {
        return Key == other.Key;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (obj.GetType() != GetType())
            return false;

        var other = (EnumObject)obj;
        return Key == other.Key;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public static bool operator ==(EnumObject? left, EnumObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(EnumObject? left, EnumObject? right)
    {
        if (left is null && right is null)
            return false;

        if (left is null || right is null)
            return true;

        return !left.Equals(right);
    }

    public static implicit operator string(EnumObject obj)
    {
        return obj.Key;
    }
}