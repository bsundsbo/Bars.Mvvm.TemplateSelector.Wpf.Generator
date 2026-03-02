using System.Linq;
using Microsoft.CodeAnalysis;
using System;
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

                predicate: static (_, _) => true, // accept all — filtered later
                transform: TransformAnalyzedTypes)
            .Where(static pair => pair.ClassSymbol is not null && pair.AttributeModel is not null)
            .Collect();

        context.RegisterSourceOutput(attributedClasses, static (ctx, items) =>
        {
            foreach (var (classSymbol, attributeModel, isNullableContext) in items!)
            {
                try
                {
                    string selectorClassCode = AutoTemplateSelectorClassGenerator.CreateClass(classSymbol);
                    ctx.AddSource($"{classSymbol.Name}.g.cs", selectorClassCode);

                    var resourceDictionaryClassCode = AutoTemplateSelectorDictionaryClassGenerator.CreateClass(attributeModel.ResourceDictionarySymbol);
                    ctx.AddSource($"{classSymbol.Name}.{attributeModel.ResourceDictionary}.g.cs", resourceDictionaryClassCode);

                    string sourceCode = AutoTemplateSelectorResourceKeyClassGenerator.GenerateResourceKeys(classSymbol);
                    ctx.AddSource($"{classSymbol.Name}.{attributeModel.ResourceDictionaryKey}.g.cs", sourceCode);

                    ctx.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor("GEN001", "Found Dictionary", $"Class {classSymbol.Name} references ResourceDictionary: {attributeModel.ResourceDictionary} and ResourceKeys {attributeModel.ResourceDictionaryKey}", "Generator", DiagnosticSeverity.Info, true),
                        null));
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
    private static (INamedTypeSymbol ClassSymbol, AttributeModel AttributeModel, bool IsNullableContext) TransformAnalyzedTypes(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
#pragma warning restore SA1414
    {
        try
        {
            if (ctx.TargetSymbol is not INamedTypeSymbol classSymbol
                || !ClassParser.IsClass(classSymbol)
                || !ClassParser.IsPartial(classSymbol)
                || !ClassParser.IsTemplateSelector(classSymbol))
            {
                return default;
            }

            var attr = ctx.Attributes
                .Select(AttributeParser.GetAttributeDetails)
                .FirstOrDefault(aa => aa != null);
            if (attr is null)
            {
                return default;
            }

            var nullableContext = NullableContextUtilities.GetEffectiveNullableContext(ctx.SemanticModel, ctx.TargetNode);
            bool isNullable = nullableContext == NullableContextOptions.Enable;
            return  (classSymbol, attr, isNullable);

        }
        catch (Exception)
        {
            return default((INamedTypeSymbol, AttributeModel, bool));
        }
    }
}
