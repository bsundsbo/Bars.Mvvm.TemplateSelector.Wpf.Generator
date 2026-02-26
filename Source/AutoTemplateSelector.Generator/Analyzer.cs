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
    private static readonly DiagnosticDescriptor _rule02 = new("ATS02", "Missing or incorrect base type", "Class must derive from DataTemplate or ItemControlTemplate", _category,
        DiagnosticSeverity.Error, isEnabledByDefault: true, description: "Class must derive from DataTemplate or ItemControlTemplate.");

    private static readonly DiagnosticDescriptor _rule03 = new(
        "ATS03",
        "Missing x:Class on ResourceDictionary XAML",
        "The ResourceDictionary type passed to AutoTemplateSelectorAttribute must be declared as <ResourceDictionary x:Class=\"{0}\"",
        _category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Ensures the ResourceDictionary type argument corresponds to a XAML file with x:Class so WPF can connect generated components.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return [_rule01, _rule02, _rule03]; } }

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

        var autoTemplateSelectorAttribute = AttributeParser.GetAutoTemplateSelectorAttributeData(namedType);
        if (autoTemplateSelectorAttribute is null)
        {
            return;
        }

        if (!IsPartial(namedType))
        {
            var diagnostic = Diagnostic.Create(_rule01, context.Symbol.Locations.First(), string.Empty);
            context.ReportDiagnostic(diagnostic);
        }

        if (!HasValidBaseClass(namedType))
        {
            var diagnostic = Diagnostic.Create(_rule02, context.Symbol.Locations.First(), "Base class must be ItemTemplateSelector or DataTemplateSelector");
            context.ReportDiagnostic(diagnostic);
        }

        // Heuristic: if the ResourceDictionary has x:Class, the generated code-behind type implements IComponentConnector.
        // This avoids parsing XAML in the analyzer.
        if (!TryGetResourceDictionaryTypeArgument(autoTemplateSelectorAttribute, out var resourceDictionaryType))
        {
            return;
        }

        var componentConnector = context.Compilation.GetTypeByMetadataName("System.Windows.Markup.IComponentConnector");
        if (componentConnector is not null && !ImplementsInterface(resourceDictionaryType, componentConnector))
        {
            var location =
                TryGetFirstTypeofTypeLocation(autoTemplateSelectorAttribute, context)
                ?? context.Symbol.Locations.First();

            context.ReportDiagnostic(Diagnostic.Create(_rule03, location, resourceDictionaryType.ToDisplayString()));
        }
    }

    private static Location? TryGetFirstTypeofTypeLocation(AttributeData attributeData, SymbolAnalysisContext context)
    {
        if (attributeData.ApplicationSyntaxReference?.GetSyntax(context.CancellationToken) is not AttributeSyntax attributeSyntax)
        {
            return null;
        }

        // Handles: [AutoTemplateSelector(typeof(Foo))]
        var firstArgExpression = attributeSyntax.ArgumentList?.Arguments.FirstOrDefault()?.Expression;
        if (firstArgExpression is TypeOfExpressionSyntax typeOfExpression)
        {
            return typeOfExpression.Type.GetLocation();
        }

        // Handles: [AutoTemplateSelector(Foo)] if someone ever changes the attribute API to accept a type name differently
        // or [AutoTemplateSelector(typeof(Foo), ...)] still covered above.
        return firstArgExpression?.GetLocation();
    }

    public static bool HasValidBaseClass(INamedTypeSymbol? namedType)
    {
        if (namedType?.BaseType is null)
        {
            return false;
        }

        // Check if the base class is a valid ResourceDictionary or DataTemplateSelector
        bool hasCorrectBaseClass = namedType.BaseType.ToDisplayString() == "System.Windows.Controls.ItemContainerTemplateSelector" ||
               namedType.BaseType.ToDisplayString() == "System.Windows.Controls.DataTemplateSelector";
        if (hasCorrectBaseClass)
        {
            return true;
        }

        return HasValidBaseClass(namedType.BaseType);
    }

    public static bool IsClass(INamedTypeSymbol? namedType)
    {
        return namedType?.TypeKind == TypeKind.Class;
    }

    private static bool TryGetResourceDictionaryTypeArgument(AttributeData autoTemplateSelectorAttribute, out INamedTypeSymbol resourceDictionaryType)
    {
        resourceDictionaryType = null!;

        if (autoTemplateSelectorAttribute.ConstructorArguments.Length < 1)
        {
            return false;
        }

        var arg = autoTemplateSelectorAttribute.ConstructorArguments[0];
        if (arg.Kind != TypedConstantKind.Type || arg.Value is not INamedTypeSymbol typeSymbol)
        {
            return false;
        }

        resourceDictionaryType = typeSymbol;
        return true;
    }

    private static bool ImplementsInterface(INamedTypeSymbol type, INamedTypeSymbol interfaceType)
    {
        return type.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interfaceType));
    }

    public static bool IsPartial(INamedTypeSymbol? namedType)
    {
        return namedType?.DeclaringSyntaxReferences.First().GetSyntax() is TypeDeclarationSyntax typeDeclaration &&
               typeDeclaration.Modifiers.Any(x => x.IsKeyword() && x.IsKind(SyntaxKind.PartialKeyword));
    }
}
