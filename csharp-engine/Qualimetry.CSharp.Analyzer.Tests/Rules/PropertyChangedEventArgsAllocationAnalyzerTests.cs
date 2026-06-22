using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Style;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PropertyChangedEventArgsAllocationAnalyzerTests
{
    [Fact]
    public Task PerNotificationAllocation_IsReported()
    {
        const string source = """
            using System.ComponentModel;

            public class ViewModel : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler PropertyChanged;

                public void RaiseName()
                {
                    PropertyChanged?.Invoke(this, {|qa_style_property_changed_event_args_allocation:new PropertyChangedEventArgs("Name")|});
                }
            }
            """;

        return CSharpAnalyzerVerifier<PropertyChangedEventArgsAllocationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task CachedArgs_IsClean()
    {
        const string source = """
            using System.ComponentModel;

            public class ViewModel : INotifyPropertyChanged
            {
                private static readonly PropertyChangedEventArgs NameArgs = new PropertyChangedEventArgs("Name");

                public event PropertyChangedEventHandler PropertyChanged;

                public void RaiseName()
                {
                    PropertyChanged?.Invoke(this, NameArgs);
                }
            }
            """;

        return CSharpAnalyzerVerifier<PropertyChangedEventArgsAllocationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task NameofArgument_IsReported()
    {
        const string source = """
            using System.ComponentModel;

            public class ViewModel : INotifyPropertyChanged
            {
                public string Name { get; set; }

                public event PropertyChangedEventHandler PropertyChanged;

                public void RaiseName()
                {
                    PropertyChanged?.Invoke(this, {|qa_style_property_changed_event_args_allocation:new PropertyChangedEventArgs(nameof(Name))|});
                }
            }
            """;

        return CSharpAnalyzerVerifier<PropertyChangedEventArgsAllocationAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task DynamicPropertyName_IsClean()
    {
        const string source = """
            using System.ComponentModel;
            using System.Runtime.CompilerServices;

            public class ViewModel : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler PropertyChanged;

                public void Raise([CallerMemberName] string propertyName = null)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            }
            """;

        return CSharpAnalyzerVerifier<PropertyChangedEventArgsAllocationAnalyzer>.VerifyAsync(source);
    }
}
