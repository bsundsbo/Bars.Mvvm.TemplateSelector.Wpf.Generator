// C#
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
namespace AutoTemplateSelector.Generator;
internal static class NullableContextUtilities
{
    /// <summary>
    /// Returns the effective NullableContextOptions at the given syntax node using the provided SemanticModel.
    /// Uses the SemanticModel's nullable context determination (which respects #nullable directives),
    /// and falls back to the compilation options if the model doesn't provide a definitive answer.
    /// </summary>
    public static NullableContextOptions GetEffectiveNullableContext(SemanticModel semanticModel, SyntaxNode node)
    {
        if (semanticModel == null)
        {
            throw new ArgumentNullException(nameof(semanticModel));
        }

        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        // Try to use the SemanticModel helper which takes into account #nullable directives in the tree.
        // Some Roslyn versions provide GetNullableContext(SyntaxNode); others provide GetNullableContext(int position).
        try
        {
            // Preferred: Get context by node (respects directives and position)
            var ctx = semanticModel.GetNullableContext(node.SpanStart);
            return MapRoslynNullableContextToOptions(ctx);
        }
        catch (MissingMethodException)
        {
            // If GetNullableContext(SyntaxNode) not available, fall back to position-based API:
            try
            {
                var pos = node.SpanStart; // SpanStart is available on SyntaxNode
                var ctx2 = semanticModel.GetNullableContext(pos);
                return MapRoslynNullableContextToOptions(ctx2);
            }
            catch
            {
                // Fall through to compilation fallback below
            }
        }
        catch
        {
            // Fall through to compilation fallback below
        }

        // Fallback: derive from the compilation options
        var compilation = semanticModel.Compilation;
        if (compilation is CSharpCompilation csCompilation)
        {
            return csCompilation.Options.NullableContextOptions;
        }

        // Conservative default: nullable disabled
        return NullableContextOptions.Disable;
    }

    private static NullableContextOptions MapRoslynNullableContextToOptions(object roslynContext)
    {
        // Roslyn's GetNullableContext may return NullableContext (internal) or NullableContextOptions depending on version.
        // Try to map both possibilities.
        if (roslynContext is NullableContextOptions nco)
        {
            return nco;
        }

        if(roslynContext is NullableContext nc)
        {
            // Map NullableContext to NullableContextOptions
            bool warnings = (nc & NullableContext.WarningsEnabled) == NullableContext.WarningsEnabled;
            bool annotations = (nc & NullableContext.AnnotationsEnabled) == NullableContext.AnnotationsEnabled;

            if (warnings && annotations)
            {
                return NullableContextOptions.Enable;
            }

            if (warnings)
            {
                return NullableContextOptions.Warnings;
            }

            if (annotations)
            {
                return NullableContextOptions.Annotations;
            }

            return NullableContextOptions.Disable;
        }

        if (roslynContext is int intVal)
        {
            // int-based enum -> try to cast
            return (NullableContextOptions)intVal;
        }

        // As a safety fallback treat unknown as Disable
        return NullableContextOptions.Disable;
    }

    /// <summary>
    /// Convenience: returns true if the given ITypeSymbol is declared/annotated as nullable (T?).
    /// </summary>
    public static bool IsAnnotatedNullable(ITypeSymbol typeSymbol)
    {
        if (typeSymbol == null)
        {
            throw new ArgumentNullException(nameof(typeSymbol));
        }

        return typeSymbol.NullableAnnotation == NullableAnnotation.Annotated;
    }
}