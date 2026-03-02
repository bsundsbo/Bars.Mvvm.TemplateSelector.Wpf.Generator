using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AutoTemplateSelector.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class AutoTemplateSelectorDecoratedTypeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [DiagnosticDescriptors.Rule01, DiagnosticDescriptors.Rule02];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSymbolAction(AnalyzeDecoratedType, SymbolKind.NamedType);
    }

    private static void AnalyzeDecoratedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol namedType)
        {
            return;
        }

        if (!ClassParser.IsClass(namedType))
        {
            return;
        }

        var autoTemplateSelectorAttribute = AttributeParser.GetAutoTemplateSelectorAttributeData(namedType);
        if (autoTemplateSelectorAttribute is null)
        {
            // Not decorated with attribute; exit
            return;
        }

        if (!ClassParser.IsPartial(namedType))
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.Rule01, context.Symbol.Locations.First(), string.Empty);
            context.ReportDiagnostic(diagnostic);
        }

        if (!ClassParser.IsTemplateSelector(namedType))
        {
            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.Rule02, context.Symbol.Locations.First(), "Base class must be ItemTemplateSelector or DataTemplateSelector");
            context.ReportDiagnostic(diagnostic);
        }
    }

}
