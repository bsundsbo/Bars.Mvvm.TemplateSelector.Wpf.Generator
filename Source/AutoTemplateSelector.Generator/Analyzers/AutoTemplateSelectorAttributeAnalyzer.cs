using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AutoTemplateSelector.Generator;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class AutoTemplateSelectorAttributeAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [DiagnosticDescriptors.Rule03, DiagnosticDescriptors.Rule04, DiagnosticDescriptors.Rule05];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSymbolAction(AnalyzeAttributeUsage, SymbolKind.NamedType);
    }

    private static void AnalyzeAttributeUsage(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol namedType)
        {
            return;
        }

        var attributeData = AttributeParser.GetAutoTemplateSelectorAttributeData(namedType);
        if (attributeData is null)
        {
            return;
        }

        var resourceDictionary = AttributeParser.GetNamedArgumentValue(attributeData, nameof(AutoTemplateSelectorAttribute.ResourceDictionary));
        var resourceKeys = AttributeParser.GetNamedArgumentValue(attributeData, nameof(AutoTemplateSelectorAttribute.ResourceKeys));

        if(resourceDictionary == null || resourceKeys == null)
        {
            var missing = string.Join(", ", new[]
            {
                resourceDictionary == null ? nameof(AutoTemplateSelectorAttribute.ResourceDictionary) : null,
                resourceKeys == null ? nameof(AutoTemplateSelectorAttribute.ResourceKeys) : null
            }.Where(x => x is not null));
            var location =
                TryGetAttributeLocation(attributeData, context)
                ?? context.Symbol.Locations.First();

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule04, location, missing));
            return;
        }

        ValidateAssignedTypes(context, attributeData, resourceDictionary, resourceKeys);
    }

    private static void ValidateAssignedTypes(
        SymbolAnalysisContext context,
        AttributeData attributeData,
        INamedTypeSymbol? resourceDictionaryType,
        INamedTypeSymbol? resourceDictionaryKeyType)
    {
        // Intentionally empty for now.
        // Later: add rules like:
        // - ResourceDictionary derives from System.Windows.ResourceDictionary
        // - ResourceDictionaryKey is static partial class
        // - etc.
        _ = context;
        _ = attributeData;
        _ = resourceDictionaryType;
        _ = resourceDictionaryKeyType;

        ValidateResourceDictionaryType(context, attributeData, resourceDictionaryType);
    }

    private static void ValidateResourceDictionaryType(SymbolAnalysisContext context, AttributeData attributeData,
        INamedTypeSymbol? resourceDictionaryType)
    {
        if (resourceDictionaryType is null)
        {
            return;
        }

        if (!ClassParser.HasValidBaseClass(resourceDictionaryType, "System.Windows.ResourceDictionary"))
        {
            var location =
                TryGetNamedTypeofLocation(attributeData, nameof(AutoTemplateSelectorAttribute.ResourceDictionary), context)
                ?? TryGetAttributeLocation(attributeData, context)
                ?? context.Symbol.Locations.First();

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule05, location, resourceDictionaryType.ToDisplayString()));
            return;
        }

        // Heuristic: if the ResourceDictionary has x:Class, the generated code-behind type implements IComponentConnector.
        // This avoids parsing XAML in the analyzer.
        var componentConnector = context.Compilation.GetTypeByMetadataName("System.Windows.Markup.IComponentConnector");
        if (componentConnector is not null && !ImplementsInterface(resourceDictionaryType, componentConnector))
        {
            var location =
                TryGetNamedTypeofLocation(attributeData, nameof(AutoTemplateSelectorAttribute.ResourceDictionary), context)
                ?? TryGetAttributeLocation(attributeData, context)
                ?? context.Symbol.Locations.First();

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.Rule03, location, resourceDictionaryType.ToDisplayString()));
        }
    }

    private static bool ImplementsInterface(INamedTypeSymbol type, INamedTypeSymbol interfaceType)
    {
        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interfaceType));
    }

    private static Location? TryGetAttributeLocation(AttributeData attributeData, SymbolAnalysisContext context)
    {
        return attributeData.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) switch
        {
            AttributeSyntax a => a.GetLocation(),
            _ => null,
        };
    }

    private static Location? TryGetNamedTypeofLocation(AttributeData attributeData, string argumentName, SymbolAnalysisContext context)
    {
        if (attributeData.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) is not AttributeSyntax attributeSyntax)
        {
            return null;
        }

        var args = attributeSyntax.ArgumentList?.Arguments;
        if (args is null)
        {
            return null;
        }

        foreach (var arg in args)
        {
            if (arg.NameEquals?.Name.Identifier.ValueText != argumentName)
            {
                continue;
            }

            return arg.Expression is TypeOfExpressionSyntax toe
                ? toe.Type.GetLocation()
                : arg.Expression.GetLocation();
        }

        return null;
    }
}