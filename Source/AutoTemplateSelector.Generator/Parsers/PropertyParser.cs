using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AutoTemplateSelector.Generator;

/// <summary>
/// This class parses properties of a class, and gets relevant details about them.
/// </summary>
internal static class PropertyParser
{
    public static ImmutableArray<IPropertySymbol> GetRelevantProperties(INamedTypeSymbol? classSymbol)
    {
        ImmutableArray<IPropertySymbol> ret = [];
        if (classSymbol == null)
        {
            return ret;
        }

        var properties = classSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p =>  p is {IsStatic: false, IsReadOnly: false})
            .ToImmutableArray();
        return properties;
    }
}

