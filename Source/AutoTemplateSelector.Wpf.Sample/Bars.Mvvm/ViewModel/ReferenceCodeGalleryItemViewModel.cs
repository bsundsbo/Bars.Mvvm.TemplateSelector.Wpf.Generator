using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace AutoTemplateSelector.Wpf.Sample;

/// <summary>
/// Implementation of <see cref="BarGalleryItemViewModel{T}"/> for <see cref="ReferenceCode"/>.
/// </summary>
/// <param name="referenceCode"></param>
public class ReferenceCodeGalleryItemViewModel(ReferenceCode referenceCode) : BarGalleryItemViewModel<ReferenceCode>(referenceCode)
{
    public string Code => Value.Code;
    protected override string CoerceLabel()
    {
        return Value.Name;
    }
}