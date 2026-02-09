#nullable disable
using ActiproSoftware.Windows.Controls.Bars.Mvvm;
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

namespace AutoTemplateSelector.Wpf.Sample;

/// <summary>
/// This class has to be partial, and decorated with <see cref="AutoTemplateSelector"/>,
/// The type passed to the constructor of the attribute must be a resource dictionary with x:Class referencing this class in
/// the XAML.
/// <para>
/// The properties of <see cref="NonNullableDataTemplateSelector"/> are generated into <see cref="NonNullableDataTemplateSelectorResourceDictionary"/>
/// and mapped within the generated partial class <see cref="NonNullableDataTemplateSelector"/> so they can be returned in the
/// overriden <see cref="SelectTemplate"/> method.
/// </para>
/// </summary>
[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public partial class NonNullableDataTemplateSelector : BarControlTemplateSelector
{
    /// <summary>
    /// The template used for a blue button.
    /// </summary>
    public ItemContainerTemplate BlueButtonTemplate { get; set; }

    /// <summary>
    /// The template used for a red button.
    /// </summary>
    public ItemContainerTemplate RedButtonTemplate { get; set; }

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
