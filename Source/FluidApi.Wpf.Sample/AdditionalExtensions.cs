using ActiproSoftware.Windows.Controls;
using ActiproSoftware.Windows.Controls.Bars;
using ActiproSoftware.Windows.Controls.Bars.Mvvm;
using MahApps.Metro.IconPacks;
using System.Windows.Media;

namespace Bars.Mvvm.FluidGenerator.Sample;

public static class AdditionalExtensions
{
    public static T WithImages<T>(this T target, BarImageProvider imageProvider)
        where T : BarKeyedObjectViewModelBase, IHasVariantImages
    {
        target.LargeImageSource = imageProvider.GetImageSource(target.Key, BarImageSize.Large);
        target.MediumImageSource = imageProvider.GetImageSource(target.Key, BarImageSize.Medium);
        target.SmallImageSource = imageProvider.GetImageSource(target.Key, BarImageSize.Small);
        return target;
    }

    public static ImageSource? CreateImage(this BarImageOptions _, PackIconMaterialKind kind)
    {
        return new MaterialImageExtension(kind)
            // ReSharper disable once NullableWarningSuppressionIsUsed
            .ProvideValue(null!) as ImageSource;
    }
}
