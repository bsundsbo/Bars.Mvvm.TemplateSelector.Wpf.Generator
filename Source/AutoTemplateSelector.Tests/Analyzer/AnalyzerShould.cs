using Microsoft.CodeAnalysis.Testing;
using Xunit;
using AnalyzeCS = AutoTemplateSelector.Tests.WpfAnalyzerFixture<AutoTemplateSelector.Generator.AutoTemplateSelectorDecoratedTypeAnalyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<AutoTemplateSelector.Generator.AutoTemplateSelectorDecoratedTypeAnalyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
namespace AutoTemplateSelector.Tests;

public class AnalyzerShould
{
    [Fact]
    public async Task ReportsDiagnosticError_ATS01_NotPartial()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;
using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary, System.Windows.Markup.IComponentConnector
{
    public void InitializeComponent() {}
    void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {}
}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public class NonNullableDataTemplateSelector : BarControlTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic("ATS01").WithLocation(15, 14).WithArguments(string.Empty);
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ReportsDiagnosticError_ATS02_InvalidBaseClass()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls.Primitives;
using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary, System.Windows.Markup.IComponentConnector
{
    public void InitializeComponent() {}
    void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {}
}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public partial class NonNullableDataTemplateSelector : Selector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic("ATS02").WithLocation(16, 22).WithArguments(string.Empty);
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ReportsDiagnosticError_ATS02_MissingBaseClass()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;

using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary, System.Windows.Markup.IComponentConnector
{
    public void InitializeComponent() {}
    void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {}
}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public partial class NonNullableDataTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic("ATS02").WithSpan(16, 22, 16, 53).WithArguments("Base class must be ItemTemplateSelector or DataTemplateSelector");
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task ReportsDiagnosticError_ATS03_MissingXClassAttribute()
    {
        string code = @"
using AutoTemplateSelector.Generator;
using System.Windows;
using System.Windows.Controls;

using ActiproSoftware.Windows.Controls.Bars.Mvvm;

namespace Test;

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary
{
}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public partial class NonNullableDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic("ATS03")
            .WithSpan(14, 30, 14, 79)
            .WithArguments("Test.NonNullableDataTemplateSelectorResourceDictionary");
        test.ExpectedDiagnostics.Add(expected);
        await test.RunAsync(TestContext.Current.CancellationToken);
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

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary, System.Windows.Markup.IComponentConnector
{
public void InitializeComponent() {}
void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {}
}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
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
