using System.Linq;
using Microsoft.CodeAnalysis;

namespace AutoTemplateSelector.Generator;

/// <summary>
/// Helper class for parsing <see cref="AutoTemplateSelectorAttribute"/>.
/// </summary>
internal static class AttributeParser
{
    /// <summary>
    /// Gets <see cref="AttributeData"/> for a class that may be decorated with the <see cref="AutoTemplateSelectorAttribute"/>,
    /// if any.
    /// </summary>
    /// <param name="namedTypeSymbol">The <see cref="INamedTypeSymbol"/> to get data from.</param>
    /// <returns><see langword="null"/> if <paramref name="namedTypeSymbol"/></returns>
    public static AttributeData? GetAutoTemplateSelectorAttributeData(INamedTypeSymbol namedTypeSymbol)
    {
        var attributeData = namedTypeSymbol.GetAttributes().FirstOrDefault(x =>
            x.AttributeClass?.Name is nameof(AutoTemplateSelectorAttribute) &&
            x.AttributeClass.ContainingNamespace.ToDisplayString() == typeof(AutoTemplateSelectorAttribute).Namespace);
        return attributeData;
    }

    public static AttributeModel? GetAttributeDetails(INamedTypeSymbol namedTypeSymbol)
    {
        return GetAttributeDetails(GetAutoTemplateSelectorAttributeData(namedTypeSymbol));
    }

    public static AttributeModel? GetAttributeDetails(AttributeData? attributeData)
    {
        if (attributeData is null)
        {
            return null;
        }

        // Create a new instance of the AttributeModel class
        // get named arguments of the attribute for the ResourceDictionary and ResourceDictionaryKey
        // assign these to a new instance of AttributeModel
        var resourceDictionaryType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
        var resourceDictionaryKeySymbol =
            GetNamedArgumentValue(attributeData, nameof(AutoTemplateSelectorAttribute.ResourceDictionaryKey));
        if (resourceDictionaryKeySymbol is null || resourceDictionaryType is null)
        {
            return null;
        }

        return new AttributeModel(resourceDictionaryType, resourceDictionaryKeySymbol);
    }

    private static INamedTypeSymbol? GetNamedArgumentValue(AttributeData attributeData, string argumentName)
    {
        return attributeData.NamedArguments.FirstOrDefault(na => na.Key == argumentName).Value.Value as INamedTypeSymbol;
    }
}

