using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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
        // get named arguments of the attrubute for ResourceDictionary and ResourceDictionaryKey
        // assign these to a new instance of AttributeModel
        var model = new AttributeModel()
        {
            ResourceDictionarySymbol = attributeData.ConstructorArguments[0].Value as ITypeSymbol,
            ResourceDictionaryKeySymbol = GetNamedArgumentValue(attributeData, nameof(AutoTemplateSelectorAttribute.ResourceDictionaryKey))
        };
        return model;
    }

    private static INamedTypeSymbol? GetNamedArgumentValue(AttributeData attributeData, string argumentName)
    {
        return attributeData.NamedArguments.FirstOrDefault(na => na.Key == argumentName).Value.Value as INamedTypeSymbol;
    }

    /// <summary>
    /// Contains the data from the <see cref="AutoTemplateSelectorAttribute"/>.
    /// </summary>
    internal record AttributeModel
    {
        /// <summary>
        /// The <see cref="INamedTypeSymbol"/> of the <see cref="AutoTemplateSelectorAttribute.ResourceDictionary"/> argument.
        /// </summary>
        public ITypeSymbol? ResourceDictionarySymbol { get; init; }
        /// <summary>
        /// The <see cref="INamedTypeSymbol"/> of the <see cref="AutoTemplateSelectorAttribute.ResourceDictionaryKey"/> argument.
        /// </summary>
        public INamedTypeSymbol? ResourceDictionaryKeySymbol { get; init; }
        public string? ResourceDictionary => ResourceDictionarySymbol?.Name;
        public string? ResourceDictionaryKey => ResourceDictionaryKeySymbol?.Name;
    }
}

