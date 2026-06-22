using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Metrics;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class TypePoorCohesionAnalyzerTests
{
    [Fact]
    public Task DisjointMethodGroups_IsReported()
    {
        const string source = """
            public sealed class {|qa_metrics_type_poor_cohesion:KitchenSink|}
            {
                private int _invoiceCount;
                private int _shipmentCount;
                private int _refundCount;
                private int _inventoryCount;

                public void CountInvoice() => _invoiceCount++;
                public void CountShipment() => _shipmentCount++;
                public void CountRefund() => _refundCount++;
                public void CountInventory() => _inventoryCount++;
            }
            """;

        return CSharpAnalyzerVerifier<TypePoorCohesionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task SharedFields_IsClean()
    {
        const string source = """
            public sealed class OrderTotals
            {
                private int _lineCount;
                private decimal _total;

                public void AddLine(decimal amount)
                {
                    _lineCount++;
                    _total += amount;
                }

                public void Reset()
                {
                    _lineCount = 0;
                    _total = 0;
                }
            }
            """;

        return CSharpAnalyzerVerifier<TypePoorCohesionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LocalsSpelledLikeFields_DoNotMaskPoorCohesion()
    {
        const string source = """
            public sealed class {|qa_metrics_type_poor_cohesion:KitchenSink|}
            {
                private int _alpha;
                private int _beta;
                private int _gamma;
                private int _delta;

                public void DoAlpha()
                {
                    int _beta = 0;
                    _alpha = _beta;
                }

                public void DoBeta()
                {
                    int _gamma = 0;
                    _beta = _gamma;
                }

                public void DoGamma()
                {
                    int _delta = 0;
                    _gamma = _delta;
                }

                public void DoDelta()
                {
                    int _alpha = 0;
                    _delta = _alpha;
                }
            }
            """;

        return CSharpAnalyzerVerifier<TypePoorCohesionAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task LocalSpelledLikeField_DoesNotCauseFalsePositive()
    {
        const string source = """
            public sealed class OrderTotals
            {
                private int _lineCount;
                private decimal _total;
                private int _itemCount;
                private decimal _tax;

                public void AddLine(decimal amount)
                {
                    _lineCount++;
                    _total += amount;
                    _itemCount++;
                    _tax += amount;
                }

                public void Reset()
                {
                    _lineCount = 0;
                    _total = 0;
                    _itemCount = 0;
                    _tax = 0;
                }

                public void Recount()
                {
                    _lineCount++;
                    _itemCount++;
                }

                public void ApplyTax(decimal rate)
                {
                    decimal _total = rate;
                    _tax = _total;
                }
            }
            """;

        return CSharpAnalyzerVerifier<TypePoorCohesionAnalyzer>.VerifyAsync(source);
    }
}
