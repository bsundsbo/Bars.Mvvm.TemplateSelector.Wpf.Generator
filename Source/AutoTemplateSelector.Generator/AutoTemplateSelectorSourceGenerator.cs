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

                predicate: static (node, _) => true, // accept all — filtered later
                transform: TransformAnalyzedTypes)
            .Where(static pair => pair.Item1 is not null && pair.Item2 is not null)
            .Collect();

        context.RegisterSourceOutput(attributedClasses, static (ctx, items) =>
        {
            foreach (var (classSymbol, dictionarySymbol) in items!)
            {
                if (classSymbol is null)
                {
                    continue;
                }

                var dictName = dictionarySymbol.ToDisplayString();

                try
                {
                    string sourceCode = AutoTemplateSelectorCodeGenerator.Instance.Generate(classSymbol, dictionarySymbol);

                    ctx.AddSource($"{classSymbol.Name}.g.cs", sourceCode);
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
    private static (INamedTypeSymbol?, INamedTypeSymbol) TransformAnalyzedTypes(GeneratorAttributeSyntaxContext ctx, CancellationToken ct)
#pragma warning restore SA1414
    {
        try
        {
            var classSymbol = ctx.TargetSymbol as INamedTypeSymbol;
            if (!Analyzer.IsClass(classSymbol) || !Analyzer.IsPartial(classSymbol) || !Analyzer.HasValidBaseClass(classSymbol))
            {
                return default;
            }

            var attr = ctx.Attributes.FirstOrDefault();
            if (attr is null || attr.ConstructorArguments.Length == 0)
            {
                return default;
            }

            var arg = attr.ConstructorArguments[0];
            return arg.Value is not INamedTypeSymbol dictType
                ? default((INamedTypeSymbol, INamedTypeSymbol))
                : (classSymbol, dictType);

        }
        catch (Exception)
        {
            return default((INamedTypeSymbol, INamedTypeSymbol));
        }
    }
}
