#nullable disable
using ActiproSoftware.Windows.Controls.Bars.Mvvm;
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

namespace TemplateSelector.Wpf.Console;

/// <summary>
/// some doc to be filled in later.
/// </summary>
[AutoTemplateSelector(typeof(ConsoleControlTemplateSelectorResourceDictionary))]
public partial class ConsoleControlTemplateSelector : BarControlTemplateSelector
{
    /// <summary>
    /// Gets or sets template for blue button.
    /// </summary>
    public ItemContainerTemplate BlueButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets template for red button.
    /// </summary>
    public ItemContainerTemplate RedButtonTemplate { get; set; }

    /// <summary>
    /// Gets or sets template for green button.
    /// </summary>
    private ItemContainerTemplate GreenButtonTemplate { get; set; }

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
