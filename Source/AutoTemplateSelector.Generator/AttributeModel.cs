using Microsoft.CodeAnalysis;

namespace AutoTemplateSelector.Generator;

/// <summary>
/// Contains the data from the <see cref="AutoTemplateSelectorAttribute"/>.
/// </summary>
/// <param name="ResourceDictionarySymbol">The <see cref="INamedTypeSymbol"/> of the <see cref="AutoTemplateSelectorAttribute.ResourceDictionary"/> argument.</param>
/// <param name="ResourceKeysSymbol">The <see cref="INamedTypeSymbol"/> of the <see cref="AutoTemplateSelectorAttribute.ResourceKeys"/> argument.</param>
internal record AttributeModel(INamedTypeSymbol ResourceDictionarySymbol, INamedTypeSymbol ResourceKeysSymbol)
{
    public string ResourceDictionary => ResourceDictionarySymbol.Name;
    public string ResourceDictionaryKey => ResourceKeysSymbol.Name;
}

