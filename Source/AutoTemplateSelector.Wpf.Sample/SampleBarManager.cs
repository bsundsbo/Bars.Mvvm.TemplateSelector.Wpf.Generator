using ActiproSoftware.Windows.Controls.Bars;
using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Bars.Mvvm.FluidGenerator.Sample;

public class SampleBarManager
{
    /// <summary>
    /// Gets the collection of control view models.
    /// </summary>
    /// <value>A <see cref="BarControlViewModelCollection"/>.</value>
    public BarControlViewModelCollection ControlViewModels { get; } = new();
    public BarImageProvider ImageProvider { get; } = new();

    public SampleBarManager()
    {
        // Register view models for controls
        RegisterControlViewModels();
    }

    private void RegisterControlViewModels()
    {
        ControlViewModels.Register(SampleControlKeys.ComboBox, CreateReferenceCodeComboBox);
        ControlViewModels.Register(SampleControlKeys.Gallery, CreateReferenceCodeGallery);
    }

    private static IHasKey CreateReferenceCodeComboBox(string key)
    {
        var items = new List<ReferenceCode>
        {
            new ("Code1", "Description for Code1"),
            new ("Code2", "Description for Code2"),
            new ("Code3", "Description for Code3")
        };

        var galleryItems = items.Select(item => new ReferenceCodeGalleryItemViewModel(item))
            .ToList();
        return new BarComboBoxViewModel(key, galleryItems)
            .WithLabel("Reference Codes")
            .WithTextPath(nameof(ReferenceCodeGalleryItemViewModel.Code))
            .WithIsReadOnly(false)
            .WithPlaceholderText("Code")
            .WithUseMenuItemAppearance()
            .WithDescription("This is a combo box with a custom template selector.")
            .WithItemTemplateSelector(new NullableGalleryItemTemplateSelector());
    }

    private static IHasKey CreateReferenceCodeGallery(string key)
    {
        var items = new List<ReferenceCode>
        {
            new ("Code1", "Description for Code1"),
            new ("Code2", "Description for Code2"),
            new ("Code3", "Description for Code3"),
            new ("Code4", "Description for Code4"),
            new ("Code5", "Description for Code5"),
            new ("Code6", "Description for Code6")
        };

        var galleryItems = items.Select(item => new ReferenceCodeGalleryItemViewModel(item))
            .ToList();
        return new BarGalleryViewModel(key, galleryItems)
            .WithLabel("Reference Codes")
            .WithToolBarItemVariantBehavior(ItemVariantBehavior.AlwaysMedium)
            .WithMinMediumRibbonColumnCount(2)
            .WithMinLargeRibbonColumnCount(2)
            .WithMaxRibbonColumnCount(8)
            .WithItemTemplateSelector(new NullableGalleryItemTemplateSelector());
    }

    public RibbonViewModel GetRibbonViewModel()
    {
        return new RibbonViewModel()
            .WithItemContainerTemplateSelector(new NonNullableDataTemplateSelector())
            .WithQuickAccessToolBarMode(RibbonQuickAccessToolBarMode.None)
            .WithGroupLabelMode(RibbonGroupLabelMode.Always)
            .WithLayoutMode(RibbonLayoutMode.Simplified)
            .WithIsApplicationButtonVisible()
            .WithApplicationButton(new RibbonApplicationButtonViewModel("ApplicationButton")
                .WithLabel("File"))
            .WithTab(new RibbonTabViewModel("Tab1")
                .WithLabel("Tab label")
                .WithDescription("Tab description")
                .WithGroup(new RibbonGroupViewModel("Edit group")
                    .WithItem(ControlViewModels[SampleControlKeys.ComboBox])
                    .WithItem(ControlViewModels[SampleControlKeys.Gallery])
                )
            );
    }
}