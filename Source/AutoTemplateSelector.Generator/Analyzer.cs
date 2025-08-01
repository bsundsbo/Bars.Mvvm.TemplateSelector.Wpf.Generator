using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AutoTemplateSelector.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class Analyzer : DiagnosticAnalyzer
{
    private const string _category = "AutoTemplateSelector";

    private static readonly DiagnosticDescriptor _rule01 = new("ATS01", "Type must be partial", "Type with AutoTemplateSelectorAttribute must be partial", _category,
        DiagnosticSeverity.Error, isEnabledByDefault: true, description: "Type must be partial with ProtoContract attribute.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return [_rule01]; } }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSymbolAction(AnalyzeTypeHierarchy, SymbolKind.NamedType);
    }

    private static void AnalyzeTypeHierarchy(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol namedType)
        {
            return;
        }

        if (!IsClass(namedType))
        {
            return;
        }

        if (!HasAutoTemplateSelectorAttribute(namedType))
        {
            return;
        }

        if(!IsPartial(namedType))
        {
            var diagnostic = Diagnostic.Create(_rule01, context.Symbol.Locations.First(), string.Empty);
            context.ReportDiagnostic(diagnostic);
        }
    }

    public static bool IsClass(INamedTypeSymbol? namedType)
    {
        return namedType?.TypeKind == TypeKind.Class;
    }

    private static bool HasAutoTemplateSelectorAttribute(INamedTypeSymbol namedType)
    {
        return namedType.GetAttributes()
            .Any(x => (x.AttributeClass?.Name is nameof(AutoTemplateSelectorAttribute)) && x.AttributeClass.ContainingNamespace.ToDisplayString() == typeof(AutoTemplateSelectorAttribute).Namespace);
    }

    public static bool IsPartial(INamedTypeSymbol? namedType)
    {
        return namedType?.DeclaringSyntaxReferences.First().GetSyntax() is TypeDeclarationSyntax typeDeclaration && typeDeclaration.Modifiers.Any(x => x.IsKeyword() && x.IsKind(SyntaxKind.PartialKeyword));
    }
}
