using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace AutoTemplateSelector.Generator.Tests;

/// <summary>
/// Verifier for WPF AutoTemplateSelector code generator.
/// </summary>
/// <typeparam name="TIncrementalGenerator"></typeparam>
public static class WpfAutoTemplateSelectorSourceGeneratorVerifier<TIncrementalGenerator> where TIncrementalGenerator : IIncrementalGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<EmptySourceGeneratorProvider, DefaultVerifier>
    {
        public Test()
        {
            TestState.AdditionalReferences.Add(
                MetadataReference.CreateFromFile(typeof(AutoTemplateSelectorAttribute).Assembly.Location));
            TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(
                typeof(ActiproSoftware.Windows.Controls.Bars.Mvvm.BarButtonViewModel).Assembly.Location));
            TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows;
        }

        protected override CompilationOptions CreateCompilationOptions()
        {
            var compilationOptions = base.CreateCompilationOptions();
            return compilationOptions.WithSpecificDiagnosticOptions(
                 compilationOptions.SpecificDiagnosticOptions.SetItems(GetNullableWarningsFromCompiler()));
        }

        public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Default;

        protected override IEnumerable<Type> GetSourceGenerators() => [typeof(TIncrementalGenerator)];

        private static ImmutableDictionary<string, ReportDiagnostic> GetNullableWarningsFromCompiler()
        {
            string[] args = { "/warnaserror:nullable" };
            var commandLineArguments = CSharpCommandLineParser.Default.Parse(args, baseDirectory: Environment.CurrentDirectory, sdkDirectory: Environment.CurrentDirectory);
            var nullableWarnings = commandLineArguments.CompilationOptions.SpecificDiagnosticOptions;

            return nullableWarnings;
        }

        protected override ParseOptions CreateParseOptions()
        {
            return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);
        }
    }
}
