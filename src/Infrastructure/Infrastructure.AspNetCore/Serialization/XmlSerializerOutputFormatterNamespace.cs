using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Infrastructure.AspNetCore.Serialization;

public sealed class XmlSerializerOutputFormatterNamespace : XmlSerializerOutputFormatter
{
    protected override void Serialize(XmlSerializer xmlSerializer, XmlWriter xmlWriter, object? value)
    {
        var emptyNamespaces = new XmlSerializerNamespaces();
        emptyNamespaces.Add(string.Empty, string.Empty);
        xmlSerializer.Serialize(xmlWriter, value, emptyNamespaces);
    }
}