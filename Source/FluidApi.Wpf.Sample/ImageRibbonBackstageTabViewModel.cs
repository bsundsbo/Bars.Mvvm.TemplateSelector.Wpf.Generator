using ActiproSoftware.Windows.Controls.Bars.Mvvm;
using System.Windows.Media;

namespace Bars.Mvvm.FluidGenerator.Sample;

public class ImageRibbonBackstageTabViewModel(string key) : RibbonBackstageTabViewModel(key), IHasVariantImages
{
    ImageSource IHasVariantImages.MediumImageSource
    {
        get => null!;
        set { /* ignore */}
    }
}