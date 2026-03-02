using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace AutoTemplateSelector.Generator;

public static class ClassParser
{
    public static bool IsPartial(INamedTypeSymbol? namedType)
    {
        return namedType?.DeclaringSyntaxReferences.First().GetSyntax() is TypeDeclarationSyntax typeDeclaration &&
               typeDeclaration.Modifiers.Any(x => x.IsKeyword() && x.IsKind(SyntaxKind.PartialKeyword));
    }

    public static bool IsClass(INamedTypeSymbol? namedType)
    {
        return namedType?.TypeKind == TypeKind.Class;
    }

    public static bool HasValidBaseClass(INamedTypeSymbol? namedType, params string[] baseClasses)
    {
        if (namedType?.BaseType is null)
        {
            return false;
        }

        if (baseClasses.Contains(namedType.BaseType.ToDisplayString()))
        {
            return true;
        }

        return HasValidBaseClass(namedType.BaseType, baseClasses);
    }

    public static bool IsTemplateSelector(INamedTypeSymbol? namedType)
    {
        return HasValidBaseClass(namedType, "System.Windows.Controls.ItemContainerTemplateSelector", "System.Windows.Controls.DataTemplateSelector");
    }
}
