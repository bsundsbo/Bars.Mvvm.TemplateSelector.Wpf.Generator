namespace AutoTemplateSelector.Generator;
using System;

/// <summary>
/// Attribute to mark a class as a TemplateSelector that should be used to generate ResourceDictionary,
/// ResourceKeys and map these within the ResourceDictionary.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoTemplateSelectorAttribute : Attribute
{
    /// <summary>
    /// The type of the resource dictionary that contains the templates.
    /// </summary>
    public Type? ResourceDictionary { get; set; }

    /// <summary>
    /// The type of the <see langword="partial"/> <see langword="static"/> class to generate the resource keys for.
    /// </summary>
    public Type? ResourceKeys { get; set; }
}
