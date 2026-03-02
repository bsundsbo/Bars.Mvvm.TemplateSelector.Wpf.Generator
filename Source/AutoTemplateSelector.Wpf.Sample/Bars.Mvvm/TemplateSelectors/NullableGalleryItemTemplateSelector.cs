using ActiproSoftware.Windows.Controls.Bars.Mvvm;
using AutoTemplateSelector.Generator;
using System.Windows;

namespace AutoTemplateSelector.Wpf.Sample;

/// <summary>
/// This sample demonstrates how to use the <see cref="AutoTemplateSelector"/> attribute
/// to generate a DataTemplateSelector for a gallery item.
/// </summary>
[AutoTemplateSelector(
    ResourceKeys = typeof(TemplateSelectorResourceKeys),
    ResourceDictionary = typeof(NullableGalleryTemplateSelectorResourceDictionary))]
public partial class NullableGalleryItemTemplateSelector : BarGalleryItemTemplateSelector
{
    /// <summary>
    /// Template for a reference code gallery item.
    /// </summary>
    public DataTemplate? ReferenceCodeTemplate { get; set; }

    /// <summary>
    /// Template for a reference code gallery item when displayed as a menu item.
    /// </summary>
    public DataTemplate? ReferenceCodeMenuItemTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        bool isMenuItem = PrefersMenuItemAppearance(item, container);
        if (item is ReferenceCodeGalleryItemViewModel)
        {
            return isMenuItem ? ReferenceCodeMenuItemTemplate : ReferenceCodeTemplate;
        }

        return base.SelectTemplate(item, container);
    }
}

