using AutoTemplateSelector.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace AutoTemplateSelector.Tests;

/// <summary>
/// Fixture for analyzing code snippets in tests.
/// </summary>
/// <typeparam name="TAnalyzer"></typeparam>
/// <typeparam name="TVerifier"></typeparam>
internal class WpfAnalyzerFixture<TAnalyzer, TVerifier> : CSharpAnalyzerTest<TAnalyzer, TVerifier>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TVerifier : IVerifier, new()
{
    public WpfAnalyzerFixture()
    {
        TestState.AdditionalReferences.Add(
            MetadataReference.CreateFromFile(typeof(AutoTemplateSelectorAttribute).Assembly.Location));
        TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(
            typeof(ActiproSoftware.Windows.Controls.Bars.Mvvm.BarButtonViewModel).Assembly.Location));
        TestState.ReferenceAssemblies = ReferenceAssemblies.Net.Net90Windows;
    }
}