using Microsoft.CodeAnalysis;

namespace AutoTemplateSelector.Generator;

/// <summary>
/// Contains the data from the <see cref="AutoTemplateSelectorAttribute"/>.
/// </summary>
/// <param name="ResourceDictionarySymbol">The <see cref="INamedTypeSymbol"/> of the <see cref="AutoTemplateSelectorAttribute.ResourceDictionary"/> argument.</param>
/// <param name="ResourceDictionaryKeySymbol">The <see cref="INamedTypeSymbol"/> of the <see cref="AutoTemplateSelectorAttribute.ResourceDictionaryKey"/> argument.</param>
internal record AttributeModel(INamedTypeSymbol ResourceDictionarySymbol, INamedTypeSymbol ResourceDictionaryKeySymbol)
{
    public string ResourceDictionary => ResourceDictionarySymbol.Name;
    public string ResourceDictionaryKey => ResourceDictionaryKeySymbol.Name;
}

