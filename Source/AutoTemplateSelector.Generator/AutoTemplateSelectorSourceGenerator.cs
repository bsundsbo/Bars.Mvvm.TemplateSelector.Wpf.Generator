using System.Linq;
using Microsoft.CodeAnalysis;
using System;
using System.Text;
using System.Threading;

namespace AutoTemplateSelector.Generator;

/// <summary>
/// Source generator for generating ResourceKeys and mapping them to the DataTemplateSelector for less fuzz when creating new templates.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class AutoTemplateSelectorSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        string attributeName = typeof(AutoTemplateSelectorAttribute).FullName ?? string.Empty;
        var attributedClasses = context.SyntaxProvider
            .ForAttributeWithMetadataName(fullyQualifiedMetadataName: attributeName,

                predicate: static (node, _) => true, // accept all — filtered later
                transform: TransformAnalyzedTypes)
            .Where(static pair => pair.ClassSymbol is not null && pair.DictionaryType is not null)
            .Collect();

        context.RegisterSourceOutput(attributedClasses, static (ctx, items) =>
        {
            foreach (var (classSymbol, dictionarySymbol, isNullableContext) in items!)
            {
                if (classSymbol is null)
                {
                    continue;
                }

                var dictName = dictionarySymbol.ToDisplayString();

                try
                {
                    string selectorClassCode = AutoTemplateSelectorClassGenerator.CreateClass(classSymbol);
                    ctx.AddSource($"{classSymbol.Name}.g.cs", selectorClassCode);

                    var resourceDictionaryClassCode = AutoTemplateSelectorDictionaryClassGenerator.CreateClass(dictionarySymbol);
                    ctx.AddSource($"{classSymbol.Name}.{dictionarySymbol.Name}.g.cs", resourceDictionaryClassCode);

                    var attributeDetails = AttributeParser.GetAttributeDetails(classSymbol);
                    if (attributeDetails is not null)
                    {
                        string sourceCode = AutoTemplateSelectorResourceKeyClassGenerator.GenerateResourceKeys(classSymbol);
                        ctx.AddSource($"{classSymbol.Name}.{attributeDetails.ResourceDictionaryKey}.g.cs", sourceCode);
                    }

                    ctx.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor("GEN001", "Found Dictionary", $"Class {classSymbol.Name} references ResourceDictionary: {dictName}", "Generator", DiagnosticSeverity.Info, true),
                        classSymbol.Locations.FirstOrDefault()));
                }
                catch (Exception e)
                {
                    ctx.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor("GEN001", "Error generating type", $"Target class {classSymbol.Name} : {e}", "Generator", DiagnosticSeverity.Error, true),
                        classSymbol.Locations.FirstOrDefault()));
                }
            }
        });
    }

#pragma warning disable SA1414
    private static (INamedTypeSymbol? ClassSymbol, INamedTypeSymbol DictionaryType, bool IsNullableContext) TransformAnalyzedTypes(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
#pragma warning restore SA1414
    {
        try
        {
            var classSymbol = ctx.TargetSymbol as INamedTypeSymbol;
            if (classSymbol == null || !Analyzer.IsClass(classSymbol) || !Analyzer.IsPartial(classSymbol) || !Analyzer.HasValidBaseClass(classSymbol))
            {
                return default;
            }

            var attr = ctx.Attributes.FirstOrDefault();
            if (attr is null || attr.ConstructorArguments.Length == 0)
            {
                return default;
            }

            var arg = attr.ConstructorArguments[0];
            var nullableContext = NullableContextUtilities.GetEffectiveNullableContext(ctx.SemanticModel, ctx.TargetNode);
            bool isNullable = nullableContext == NullableContextOptions.Enable;
            return arg.Value is not INamedTypeSymbol dictType
                ? default((INamedTypeSymbol, INamedTypeSymbol, bool))
                : (classSymbol, dictType, isNullable);

        }
        catch (Exception)
        {
            return default((INamedTypeSymbol, INamedTypeSymbol, bool));
        }
    }
}
