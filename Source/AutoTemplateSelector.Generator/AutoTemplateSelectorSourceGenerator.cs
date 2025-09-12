using System.Linq;
using Microsoft.CodeAnalysis;
using System;

namespace AutoTemplateSelector.Generator;

/// <summary>
/// Source generator for generating ResourceKeys and mapping them to the DataTemplateSelector for less fuzz when creating new templates.
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class AutoTemplateSelectorSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var attributedClasses = context.SyntaxProvider
            .ForAttributeWithMetadataName(fullyQualifiedMetadataName: BarTemplateSelectorAttributeCodeGenerator.FullyQualifiedAttributeName,

                predicate: static (node, _) => true, // accept all — filtered later
                transform: static (ctx, ct) =>
                {
                    try
                    {
                        var classSymbol = ctx.TargetSymbol as INamedTypeSymbol;
                        if (!Analyzer.IsClass(classSymbol) ||
                            !Analyzer.IsPartial(classSymbol) ||
                            !Analyzer.HasValidBaseClass(classSymbol))
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
                })
            .Where(static pair => pair.Item1 is not null && pair.Item2 is not null)
            .Collect();

        context.RegisterSourceOutput(attributedClasses, static (ctx, items) =>
        {
            foreach (var (classSymbol, dictionarySymbol) in items!)
            {
                if (classSymbol is null || dictionarySymbol is null)
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
}
