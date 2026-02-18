#pragma warning disable SA1402

namespace Test;

public static partial class TemplateSelectorKeys { }

public class TemplateSelectorResourceDictionary : System.Windows.ResourceDictionary, System.Windows.Markup.IComponentConnector
{
    public void InitializeComponent() {}
    void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {}
}

[AutoTemplateSelector.Generator.AutoTemplateSelector(typeof(TemplateSelectorResourceDictionary), ResourceDictionaryKey = typeof(TemplateSelectorKeys)) ]
public partial class Selector : System.Windows.Controls.DataTemplateSelector
{
    public System.Windows.DataTemplate? BlueButtonTemplate { get; set; }
}

#pragma warning restore SA1402