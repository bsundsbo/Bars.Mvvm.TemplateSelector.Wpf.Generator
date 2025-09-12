#nullable disable
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

namespace TemplateSelector.Wpf.Console;

/// <summary>
/// some doc to be filled in later.
/// </summary>
[AutoTemplateSelector(typeof(string))]
public partial class ConsoleControlTemplateSelector : ItemContainerTemplateSelector
{
    /// <summary>
    /// Gets or sets template for blue button.
    /// </summary>
    public ItemContainerTemplate BlueButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets template for red button.
    /// </summary>
    public ItemContainerTemplate RedButtonTemplate { get; set; }

    /// <inheritdoc/>
    public override DataTemplate SelectTemplate(object item, ItemsControl parentItemsControl)
    {
        return item switch
        {
            RedBarButtonViewModel => RedButtonTemplate,
            BlueBarButtonViewModel => BlueButtonTemplate,
            _ => base.SelectTemplate(item, parentItemsControl),
        };

    }
}
