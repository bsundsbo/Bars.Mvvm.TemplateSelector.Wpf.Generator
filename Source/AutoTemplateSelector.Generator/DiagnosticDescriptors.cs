using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace AutoTemplateSelector.Generator;

/// <summary>
/// Rule IDs for <see cref="AutoTemplateSelectorAttributeAnalyzer"/> and <see cref="AutoTemplateSelectorDecoratedTypeAnalyzer"/>.
/// </summary>
internal static class DiagnosticDescriptors
{
    private const string Category = "AutoTemplateSelector";
    public static readonly DiagnosticDescriptor Rule01 = new(
        "ATS01",
        "Class must be partial",
        "Class decorated with AutoTemplateSelectorAttribute must be partial",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Type must be declared partial so the generator can emit a partial declaration.");

    public static readonly DiagnosticDescriptor Rule02 = new(
        "ATS02",
        "Incorrect base type",
        "Class decorated with AutoTemplateSelectorAttribute must derive from DataTemplate or ItemControlTemplate",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Class must derive from DataTemplateSelector or ItemContainerTemplateSelector (directly or indirectly).");

    public static readonly DiagnosticDescriptor Rule03 = new(
        "ATS03",
        "Missing x:Class on ResourceDictionary XAML",
        "The ResourceDictionary type passed to AutoTemplateSelectorAttribute must have the following attribute on the ResourceDictionary element: x:Class=\"{0}\"",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Ensures the ResourceDictionary type corresponds to a XAML file with x:Class so WPF can connect generated components.");

    public static readonly DiagnosticDescriptor Rule04 = new(
        "ATS04",
        "Required AutoTemplateSelector attribute arguments missing",
        "AutoTemplateSelectorAttribute must assign both named arguments: ResourceDictionary and ResourceDictionaryKey. Missing: '{0}'.",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Mirrors SuppressMessage requiring Justification: consumers must explicitly set both required named arguments.");

    public static readonly DiagnosticDescriptor Rule05 = new(
        "ATS05",
        "ResourceDictionary type needs to derive from System.Windows.ResourceDictionary",
        "The ResourceDictionary type passed to AutoTemplateSelectorAttribute must derive from System.Windows.ResourceDictionary",
        Category,
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Ensures the ResourceDictionary type corresponds to a XAML file with x:Class so WPF can connect generated components.");

}
