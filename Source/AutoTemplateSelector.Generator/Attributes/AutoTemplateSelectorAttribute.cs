namespace AutoTemplateSelector.Generator;
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoTemplateSelectorAttribute : Attribute
{
    private Type ResourceDictionaryType { get; }

    /// <summary>
    /// This interface marks a TemplateSelector class that is used to generate ResourceDictionary,
    /// ResourceKeys and map these within the ResourceDictionary.
    /// </summary>
    /// <param name="resourceDictionaryType">The type of the resource dictionary that contains the templates.
    /// This type has to be a ResourceDictionary XAML file with x:Class referencing the same types.
    ///</param>
    public AutoTemplateSelectorAttribute(Type resourceDictionaryType)
    {
        ResourceDictionaryType = resourceDictionaryType;
    }
}
