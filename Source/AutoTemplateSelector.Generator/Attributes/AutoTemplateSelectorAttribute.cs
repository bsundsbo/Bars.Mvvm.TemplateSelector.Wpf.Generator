namespace AutoTemplateSelector.Generator;
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoTemplateSelectorAttribute(Type resourceDictionary) : Attribute
{
    /// <summary>
    /// The type of the resource dictionary that contains the templates.
    /// </summary>
    public Type ResourceDictionary { get; init; } = resourceDictionary;

    /// <summary>
    /// The type of the partial static class to generate the resource keys in.
    /// </summary>
    public Type? ResourceDictionaryKey { get; init; }

    ///  <summary>
    ///  This interface marks a TemplateSelector class used to generate ResourceDictionary,
    ///  ResourceKeys and map these within the ResourceDictionary.
    ///  </summary>
    public AutoTemplateSelectorAttribute(Type resourceDictionary, Type resourceDictionaryKey)
        : this(resourceDictionary)
    {
    }
}
