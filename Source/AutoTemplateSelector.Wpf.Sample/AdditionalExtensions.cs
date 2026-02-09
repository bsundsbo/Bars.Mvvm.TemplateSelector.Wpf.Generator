using ActiproSoftware.Windows.Controls;
using ActiproSoftware.Windows.Controls.Bars;
using ActiproSoftware.Windows.Controls.Bars.Mvvm;
using MahApps.Metro.IconPacks;
using System.Windows.Media;

namespace AutoTemplateSelector.Wpf.Sample;

public static class AdditionalExtensions
{
    public static ImageSource? CreateImage(this BarImageOptions _, PackIconMaterialKind kind)
    {
        return new MaterialImageExtension(kind)
            // ReSharper disable once NullableWarningSuppressionIsUsed
            .ProvideValue(null!) as ImageSource;
    }
}
