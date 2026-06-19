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
}
