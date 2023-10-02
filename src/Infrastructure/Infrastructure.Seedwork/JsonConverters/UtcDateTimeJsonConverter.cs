﻿using System.Text.Json;
using System.Text.Json.Serialization;
using Infrastructure.Seedwork.DataTypes;

namespace Infrastructure.Seedwork.JsonConverters;

public sealed class UtcDateTimeJsonConverter : JsonConverter<UtcDateTime>
{
    public override UtcDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value  = reader.GetString()!;
        return (UtcDateTime)value;
    }

    public override void Write(Utf8JsonWriter writer, UtcDateTime value, JsonSerializerOptions options)
    {
        var stringDateTime = value.ToString();
        writer.WriteStringValue(stringDateTime);
    }
}