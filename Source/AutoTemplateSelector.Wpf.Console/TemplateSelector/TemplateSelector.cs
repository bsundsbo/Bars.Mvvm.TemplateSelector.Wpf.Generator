using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

namespace AutoTemplateSelector.Wpf.Console;
#pragma warning disable SA1402

[AutoTemplateSelector(ResourceDictionary = typeof(TemplateSelectorResourceDictionary), ResourceKeys = typeof(TemplateSelectorResourceKeys)) ]
public partial class TemplateSelector : DataTemplateSelector
{
    public DataTemplate? BlueButtonTemplate { get; set; }

    override public DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is BlueBarButtonViewModel)
        {
            return BlueButtonTemplate;
        }

        return null;
    }
}

public static partial class TemplateSelectorResourceKeys
{
    private static ComponentResourceKey? _somePropertyResourceKey;
    public static ComponentResourceKey SomePropertyResourceKey => _somePropertyResourceKey ??= new ComponentResourceKey(typeof(TemplateSelectorResourceKeys), nameof(SomePropertyResourceKey));
}
#pragma warning restore SA1402

