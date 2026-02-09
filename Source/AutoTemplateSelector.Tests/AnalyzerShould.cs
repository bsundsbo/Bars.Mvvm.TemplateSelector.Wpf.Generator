using Microsoft.CodeAnalysis.Testing;
using Xunit;
using AnalyzeCS = AutoTemplateSelector.Tests.AnalyzerFixture<AutoTemplateSelector.Generator.Analyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
using VerifyCS = Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerVerifier<AutoTemplateSelector.Generator.Analyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>;
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

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary
{}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public class NonNullableDataTemplateSelector : BarControlTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic("ATS01").WithLocation(12, 14).WithArguments(string.Empty);
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

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary
{}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public partial class NonNullableDataTemplateSelector : Selector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic("ATS02").WithLocation(13, 22).WithArguments(string.Empty);
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

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary
{}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public partial class NonNullableDataTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        DiagnosticResult expected = VerifyCS.Diagnostic("ATS02").WithSpan(13, 22, 13, 53).WithArguments("Base class must be ItemTemplateSelector or DataTemplateSelector");
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

public class NonNullableDataTemplateSelectorResourceDictionary : ResourceDictionary
{}

[AutoTemplateSelector(typeof(NonNullableDataTemplateSelectorResourceDictionary))]
public partial class NonNullableDataTemplateSelector : DataTemplateSelector
{
    public DataTemplate Value { get; set; }
}
";
        var test = new AnalyzeCS { TestCode = code };
        await test.RunAsync(TestContext.Current.CancellationToken);
    }
}
