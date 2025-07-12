using ActiproSoftware.Windows.Controls.Bars.Mvvm;
using System.Windows.Media;

namespace Bars.Mvvm.FluidGenerator.Sample;

public class ImageRibbonBackstageHeaderButtonViewModel(string key) : RibbonBackstageHeaderButtonViewModel(key), IHasVariantImages
{
    ImageSource IHasVariantImages.MediumImageSource
    {
        get => null!;
        set { /* ignore */}
    }

    ImageSource IHasVariantImages.LargeImageSource
    {
        get => null!;
        set { /* ignore */}
    }
}
