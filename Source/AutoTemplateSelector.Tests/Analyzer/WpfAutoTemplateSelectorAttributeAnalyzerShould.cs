using AutoTemplateSelector.Generator;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using AnalyzeCS = AutoTemplateSelector.Tests.WpfAnalyzerFixture<AutoTemplateSelector.Generator.AutoTemplateSelectorAttributeAnalyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<AutoTemplateSelector.Generator.AutoTemplateSelectorAttributeAnalyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
namespace AutoTemplateSelector.Tests;

public class WpfAutoTemplateSelectorAttributeAnalyzerShould
{
    [Fact]
    public async Task ResourceDictionary_AT05_IncorrectBaseClass()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;

public static partial class ResourceKeys {}
public class NonNullableDataTemplateSelectorResourceDictionary
{
}

[AutoTemplateSelector(ResourceDictionary = typeof(NonNullableDataTemplateSelectorResourceDictionary), ResourceKeys = typeof(ResourceKeys))]
public partial class NonNullableDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic(DiagnosticDescriptors.Rule05.Id)
            .WithSpan(15, 51, 15, 100)
            .WithMessageFormat(DiagnosticDescriptors.Rule05.MessageFormat) ;
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
        Assert.True(true);
    }

    [Fact]
    public async Task ResourceDictionary_AT03_MissingXClassAttribute()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;

public static partial class ResourceKeys {}
public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary
{
}

[AutoTemplateSelector(ResourceDictionary = typeof(NonNullableDataTemplateSelectorResourceDictionary), ResourceKeys = typeof(ResourceKeys))]
public partial class NonNullableDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic(DiagnosticDescriptors.Rule03.Id)
            .WithSpan(15, 51, 15, 100)
            .WithMessageFormat(DiagnosticDescriptors.Rule03.MessageFormat)
            .WithArguments("Test.NonNullableDataTemplateSelectorResourceDictionary");
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
        Assert.True(true);
    }

    [Fact]
    public async Task AttributeProperties_ATS04_MissingBothParameters()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;
using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;

[AutoTemplateSelector]
public partial class TemplateSelector { }
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic(DiagnosticDescriptors.Rule04.Id)
            .WithSpan(9, 2, 9, 22)
            .WithMessageFormat(DiagnosticDescriptors.Rule04.MessageFormat)
            .WithArguments("ResourceDictionary, ResourceKeys");
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AttributeProperties_ATS04_Missing_ResourceDictionary()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;
using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;
public static partial class ResourceKeys {}

[AutoTemplateSelector(ResourceKeys = typeof(ResourceKeys))]
public partial class TemplateSelector { }
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic(DiagnosticDescriptors.Rule04.Id)
            .WithSpan(10, 2, 10, 59)
            .WithMessageFormat(DiagnosticDescriptors.Rule04.MessageFormat)
            .WithArguments("ResourceDictionary");
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task AttributeProperties_ATS04_Missing_ResourceKeys()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;

namespace Test;

[AutoTemplateSelector(ResourceDictionary = typeof(ResourceDictionary))]
public partial class TemplateSelector { }
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic(DiagnosticDescriptors.Rule04.Id)
            .WithSpan(7, 2, 7, 71)
            .WithMessageFormat(DiagnosticDescriptors.Rule04.MessageFormat)
            .WithArguments("ResourceKeys");
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task NoAttribute_Verify_Success()
    {
        string code = @"
public partial class NonNullableDataTemplateSelector
{
    public object Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        test.ExpectedDiagnostics.Clear();
        await test.RunAsync(TestContext.Current.CancellationToken);
        Assert.True(true);
    }

    [Fact]
    public async Task NoError_Verify_Success()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;

public static partial class ResourceKeys {}
public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary, System.Windows.Markup.IComponentConnector
{
public void InitializeComponent() {}
void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {}
}

[AutoTemplateSelector(ResourceDictionary = typeof(NonNullableDataTemplateSelectorResourceDictionary), ResourceKeys = typeof(ResourceKeys))]
public partial class NonNullableDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        test.ExpectedDiagnostics.Clear();
        await test.RunAsync(TestContext.Current.CancellationToken);
        Assert.True(true);
    }
}
