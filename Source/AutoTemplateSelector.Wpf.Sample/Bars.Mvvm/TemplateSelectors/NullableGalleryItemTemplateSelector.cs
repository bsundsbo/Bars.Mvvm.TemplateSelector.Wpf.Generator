#nullable enable
using ActiproSoftware.Windows.Controls.Bars.Mvvm;
using AutoTemplateSelector.Generator;
using System.Windows;

namespace AutoTemplateSelector.Wpf.Sample;

/// <summary>
/// This class has to be partial, and decorated with <see cref="AutoTemplateSelector"/>,
/// The type passed to the constructor of the attribute must be a resource dictionary with x:Class referencing this class in
/// the XAML.
/// <para>
/// The properties of <see cref="NullableGalleryItemTemplateSelector"/> are generated into <see cref="CustomGalleryTemplateSelectorResourceKeys"/>
/// and mapped within the generated partial class <see cref="NullableGalleryItemTemplateSelector"/> so they can be returned in the
/// overriden <see cref="SelectTemplate"/> method.
/// </para>
/// </summary>
[AutoTemplateSelector(typeof(NullableGalleryItemTemplateSelectorResourceDictionary))]
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
