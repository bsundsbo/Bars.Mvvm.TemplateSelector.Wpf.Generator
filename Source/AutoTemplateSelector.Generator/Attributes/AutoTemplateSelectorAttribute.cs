namespace AutoTemplateSelector.Generator;
using System;

/// <summary>
/// Attribute to mark a class as a TemplateSelector that should be used to generate ResourceDictionary,
/// ResourceKeys and map these within the ResourceDictionary.
/// </summary>
/// <param name="resourceDictionary"></param>
/// <param name="resourceDictionaryKey"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoTemplateSelectorAttribute() : Attribute
{
    /// <summary>
    /// The type of the resource dictionary that contains the templates.
    /// </summary>
    public Type? ResourceDictionary { get; set; }

    /// <summary>
    /// The type of the partial static class to generate the resource keys in.
    /// </summary>
    public Type? ResourceDictionaryKey { get; set; }
}
