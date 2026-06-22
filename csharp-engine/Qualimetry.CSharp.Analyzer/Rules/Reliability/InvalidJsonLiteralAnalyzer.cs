using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Qualimetry.CSharp.Analyzer.Rules.Reliability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InvalidJsonLiteralAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableHashSet<string> ParseReceivers = ImmutableHashSet.Create(
        "JObject",
        "JArray",
        "JToken",
        "JsonDocument",
        "JsonNode",
        "Json");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptors.QCS0195);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return;
        }

        var method = memberAccess.Name.Identifier.ValueText;
        var receiver = memberAccess.Expression switch
        {
            IdentifierNameSyntax identifier => identifier.Identifier.ValueText,
            MemberAccessExpressionSyntax inner => inner.Name.Identifier.ValueText,
            _ => null,
        };

        var looksLikeJsonCall = (method == "Parse" && receiver is not null && ParseReceivers.Contains(receiver))
            || method == "DeserializeObject"
            || (method == "Deserialize" && receiver == "JsonSerializer");

        if (!looksLikeJsonCall)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol resolved)
        {
            return;
        }

        var declaringType = resolved.ContainingType?.ToDisplayString() ?? string.Empty;
        var isNewtonsoft = declaringType.StartsWith("Newtonsoft.Json", System.StringComparison.Ordinal);
        var isSystemTextJson = declaringType.StartsWith("System.Text.Json", System.StringComparison.Ordinal);
        if (!isNewtonsoft && !isSystemTextJson)
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count == 0
            || arguments[0].Expression is not LiteralExpressionSyntax literal
            || !literal.IsKind(SyntaxKind.StringLiteralExpression))
        {
            return;
        }

        if (!JsonText.IsValid(literal.Token.ValueText, lenient: isNewtonsoft))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptors.QCS0195, literal.GetLocation()));
        }
    }

    private static class JsonText
    {
        public static bool IsValid(string text, bool lenient)
        {
            var position = 0;
            SkipWhitespace(text, ref position, lenient);
            if (!ParseValue(text, ref position, lenient))
            {
                return false;
            }

            SkipWhitespace(text, ref position, lenient);
            return position == text.Length;
        }

        private static bool ParseValue(string text, ref int position, bool lenient)
        {
            if (position >= text.Length)
            {
                return false;
            }

            switch (text[position])
            {
                case '{':
                    return ParseObject(text, ref position, lenient);
                case '[':
                    return ParseArray(text, ref position, lenient);
                case '"':
                    return ParseString(text, ref position);
                case 't':
                    return ParseKeyword(text, ref position, "true");
                case 'f':
                    return ParseKeyword(text, ref position, "false");
                case 'n':
                    return ParseKeyword(text, ref position, "null");
                default:
                    return ParseNumber(text, ref position);
            }
        }

        private static bool ParseObject(string text, ref int position, bool lenient)
        {
            position++;
            SkipWhitespace(text, ref position, lenient);
            if (position < text.Length && text[position] == '}')
            {
                position++;
                return true;
            }

            while (true)
            {
                SkipWhitespace(text, ref position, lenient);
                if (position >= text.Length || text[position] != '"' || !ParseString(text, ref position))
                {
                    return false;
                }

                SkipWhitespace(text, ref position, lenient);
                if (position >= text.Length || text[position] != ':')
                {
                    return false;
                }

                position++;
                SkipWhitespace(text, ref position, lenient);
                if (!ParseValue(text, ref position, lenient))
                {
                    return false;
                }

                SkipWhitespace(text, ref position, lenient);
                if (position >= text.Length)
                {
                    return false;
                }

                if (text[position] == ',')
                {
                    position++;
                    if (lenient)
                    {
                        SkipWhitespace(text, ref position, lenient);
                        if (position < text.Length && text[position] == '}')
                        {
                            position++;
                            return true;
                        }
                    }

                    continue;
                }

                if (text[position] == '}')
                {
                    position++;
                    return true;
                }

                return false;
            }
        }

        private static bool ParseArray(string text, ref int position, bool lenient)
        {
            position++;
            SkipWhitespace(text, ref position, lenient);
            if (position < text.Length && text[position] == ']')
            {
                position++;
                return true;
            }

            while (true)
            {
                SkipWhitespace(text, ref position, lenient);
                if (!ParseValue(text, ref position, lenient))
                {
                    return false;
                }

                SkipWhitespace(text, ref position, lenient);
                if (position >= text.Length)
                {
                    return false;
                }

                if (text[position] == ',')
                {
                    position++;
                    if (lenient)
                    {
                        SkipWhitespace(text, ref position, lenient);
                        if (position < text.Length && text[position] == ']')
                        {
                            position++;
                            return true;
                        }
                    }

                    continue;
                }

                if (text[position] == ']')
                {
                    position++;
                    return true;
                }

                return false;
            }
        }

        private static bool ParseString(string text, ref int position)
        {
            position++;
            while (position < text.Length)
            {
                var c = text[position];
                if (c == '"')
                {
                    position++;
                    return true;
                }

                if (c == '\\')
                {
                    position++;
                    if (position >= text.Length)
                    {
                        return false;
                    }

                    var escape = text[position];
                    if (escape == 'u')
                    {
                        if (position + 4 >= text.Length)
                        {
                            return false;
                        }

                        for (var i = 1; i <= 4; i++)
                        {
                            if (!IsHex(text[position + i]))
                            {
                                return false;
                            }
                        }

                        position += 5;
                        continue;
                    }

                    if ("\"\\/bfnrt".IndexOf(escape) < 0)
                    {
                        return false;
                    }

                    position++;
                    continue;
                }

                if (c < ' ')
                {
                    return false;
                }

                position++;
            }

            return false;
        }

        private static bool ParseNumber(string text, ref int position)
        {
            var start = position;
            if (position < text.Length && text[position] == '-')
            {
                position++;
            }

            if (position >= text.Length || !IsDigit(text[position]))
            {
                return false;
            }

            if (text[position] == '0')
            {
                position++;
            }
            else
            {
                while (position < text.Length && IsDigit(text[position]))
                {
                    position++;
                }
            }

            if (position < text.Length && text[position] == '.')
            {
                position++;
                if (position >= text.Length || !IsDigit(text[position]))
                {
                    return false;
                }

                while (position < text.Length && IsDigit(text[position]))
                {
                    position++;
                }
            }

            if (position < text.Length && (text[position] == 'e' || text[position] == 'E'))
            {
                position++;
                if (position < text.Length && (text[position] == '+' || text[position] == '-'))
                {
                    position++;
                }

                if (position >= text.Length || !IsDigit(text[position]))
                {
                    return false;
                }

                while (position < text.Length && IsDigit(text[position]))
                {
                    position++;
                }
            }

            return position > start;
        }

        private static bool ParseKeyword(string text, ref int position, string keyword)
        {
            if (position + keyword.Length > text.Length)
            {
                return false;
            }

            for (var i = 0; i < keyword.Length; i++)
            {
                if (text[position + i] != keyword[i])
                {
                    return false;
                }
            }

            position += keyword.Length;
            return true;
        }

        private static void SkipWhitespace(string text, ref int position, bool lenient)
        {
            while (position < text.Length)
            {
                var c = text[position];
                if (c == ' ' || c == '\t' || c == '\n' || c == '\r')
                {
                    position++;
                    continue;
                }

                if (lenient && c == '/' && position + 1 < text.Length)
                {
                    var next = text[position + 1];
                    if (next == '/')
                    {
                        position += 2;
                        while (position < text.Length && text[position] != '\n')
                        {
                            position++;
                        }

                        continue;
                    }

                    if (next == '*')
                    {
                        position += 2;
                        while (position + 1 < text.Length && !(text[position] == '*' && text[position + 1] == '/'))
                        {
                            position++;
                        }

                        position = position + 1 < text.Length ? position + 2 : text.Length;
                        continue;
                    }
                }

                break;
            }
        }

        private static bool IsDigit(char c) => c >= '0' && c <= '9';

        private static bool IsHex(char c) => IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
    }
}
