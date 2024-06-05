// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Microsoft.SemanticKernel;

namespace SemanticKernel.Data.Nl2Sql.Library.Internal;

internal static class FunctionResultExtensions
{
    private static readonly HashSet<char> s_delimitersResult = new() { '\'', '"', '`' };

    public static string ParseValue(this FunctionResult result, string? label = null)
    {
        if (result == null)
        {
            return string.Empty;
        }

        var resultText = TrimDelimiters(result.GetValue<string>() ?? string.Empty);

        if (!string.IsNullOrWhiteSpace(label))
        {
            // Trim out label, if present.
            int index = resultText.IndexOf($"{label}", StringComparison.OrdinalIgnoreCase);
            if (index == 0)
            {
                resultText = resultText.Substring(index + label.Length + 1).Trim();
            }

            // Remove non-SQL lines
            if (resultText.Contains("```\n", StringComparison.OrdinalIgnoreCase)
                && resultText.Contains("```sql", StringComparison.OrdinalIgnoreCase))
            {
                resultText = resultText.Substring(resultText.IndexOf("```sql", StringComparison.Ordinal) + 6).Trim();
                resultText = resultText.Substring(0, resultText.LastIndexOf("```", StringComparison.Ordinal)).Trim();
            }
        }

        return resultText;
    }

    private static string TrimDelimiters(string expression)
    {
        for (var index = 0; index < expression.Length; ++index)
        {
            if (s_delimitersResult.Contains(expression[index]) &&
                s_delimitersResult.Contains(expression[expression.Length - index - 1]))
            {
                continue;
            }

            return expression.Substring(index, expression.Length - (index * 2)).Trim();
        }

        return expression;
    }
}
