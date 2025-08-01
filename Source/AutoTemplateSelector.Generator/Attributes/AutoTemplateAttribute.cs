namespace AutoTemplateSelector.Generator;
using System;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AutoTemplateSelectorAttribute : Attribute
{
    private Type ResourceDictionaryType { get; }

    /// <summary>
    /// This interface marks a TemplateSelector class that is used
    /// to select templates for Bar controls.
    /// </summary>
    /// <param name="resourceDictionaryType">The type of the resource dictionary that contains the templates.
    /// This type has to be a ResourceDictionary.
    ///</param>
    public AutoTemplateSelectorAttribute(Type resourceDictionaryType)
    {
        ResourceDictionaryType = resourceDictionaryType;
    }
}
