#nullable enable
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

namespace TemplateSelector.Wpf.Console;

/// <summary>
/// some doc to be filled in later.
/// </summary>
[AutoTemplateSelector(typeof(NullableTemplateSelectorResourceDictionary))]
public partial class NullableTemplateSelector : ItemContainerTemplateSelector
{
    /// <summary>
    /// Gets or sets template for red button.
    /// </summary>
    public ItemContainerTemplate? RedButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets template for green button.
    /// </summary>
    private ItemContainerTemplate GreenButtonTemplate { get; set; }

    /// <inheritdoc/>
    public override DataTemplate? SelectTemplate(object item, ItemsControl parentItemsControl)
    {
        return item switch
        {
            RedBarButtonViewModel => RedButtonTemplate,
            BlueBarButtonViewModel => GreenButtonTemplate,
            _ => base.SelectTemplate(item, parentItemsControl),
        };

    }
}
