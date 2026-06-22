using System.Threading.Tasks;
using Qualimetry.CSharp.Analyzer.Rules.Contract;
using Xunit;

namespace Qualimetry.CSharp.Analyzer.Tests.Rules;

public class PureMethodShouldBeMarkedPureAnalyzerTests
{
    [Fact]
    public Task SideEffectFreeComputation_IsReported()
    {
        const string source = """
            public class Geometry
            {
                public int {|qa_contract_pure_method_should_be_marked_pure:Square|}(int n) => n * n;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MethodAlreadyMarkedPure_IsClean()
    {
        const string source = """
            using System.Diagnostics.Contracts;

            public class Geometry
            {
                [Pure]
                public int Square(int n) => n * n;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task MethodWithInvocation_IsClean()
    {
        const string source = """
            public class Service
            {
                public int Read(int n) => System.Math.Abs(n);
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task VoidExpressionMethod_IsClean()
    {
        const string source = """
            public class Service
            {
                private int _seed;

                public void Reset() => _seed = 0;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ThrowExpressionMethod_IsClean()
    {
        const string source = """
            using System;

            public class Service
            {
                public int Get(int n) => throw new InvalidOperationException();
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task StackAllocMethod_IsClean()
    {
        const string source = """
            public class Service
            {
                public int Size(int n) => (stackalloc int[2]).Length;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task OverrideMethod_IsClean()
    {
        const string source = """
            public class Base
            {
                public virtual int {|qa_contract_pure_method_should_be_marked_pure:Square|}(int n) => 0;
            }

            public class Derived : Base
            {
                public override int Square(int n) => n * n;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }

    [Fact]
    public Task ExplicitInterfaceImplementation_IsClean()
    {
        const string source = """
            public interface ICalculator
            {
                int Square(int n);
            }

            public class Calculator : ICalculator
            {
                int ICalculator.Square(int n) => n * n;
            }
            """;

        return CSharpAnalyzerVerifier<PureMethodShouldBeMarkedPureAnalyzer>.VerifyAsync(source);
    }
}
