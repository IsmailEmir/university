using System.Text.RegularExpressions;
using MiniHttpServer.HtmlTemplating.Interfaces;

namespace MiniHttpServer.HtmlTemplating.Renderers;

public class HtmlTemplate : IHtmlTemplateRenderer
{
    public string RenderFromString(string htmlTemplate, object dataModel)
    {
        if (dataModel == null) return htmlTemplate;

        var dt = dataModel.GetType();

        if (dt == typeof(string) || dt == typeof(int) || dt == typeof(double) ||
            dt == typeof(bool) || dt == typeof(float) || dt == typeof(decimal))
        {
            return Regex.Replace(htmlTemplate, @"\$\{([^}]+)\}", _ => dataModel?.ToString() ?? "");
        }

        var result = htmlTemplate;

        result = ProcessForeachBlocks(result, dataModel);
        result = ProcessIfBlocks(result, dataModel);
        result = ReplaceVariableCore(result, dataModel, null);

        return result;
    }

    public string RenderFromFile(string filePath, object dataModel)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Шаблон не найден", filePath);
        var template = File.ReadAllText(filePath);
        return RenderFromString(template, dataModel);
    }

    public string RenderToFile(string inputFilePath, string outputFilePath, object dataModel)
    {
        var result = RenderFromFile(inputFilePath, dataModel);
        File.WriteAllText(outputFilePath, result);
        return result;
    }

    #region Задача 4: Условия $if / $else / $endif

    private static string ProcessIfBlocks(string template, object dataModel, Dictionary<string, object>? overrides = null)
    {
        var result = template;
        int ifIndex;

        while ((ifIndex = result.IndexOf("$if(")) >= 0)
        {
            var endifIndex = FindMatchingEndIf(result, ifIndex);
            if (endifIndex < 0) break;

            var condStart = ifIndex + 4;
            var condEnd = result.IndexOf(')', condStart);
            if (condEnd < 0) break;
            var condition = result.Substring(condStart, condEnd - condStart).Trim();

            var elseIndex = FindElseIndex(result, condEnd + 1, endifIndex);
            var hasElse = elseIndex >= 0 && elseIndex < endifIndex;

            var conditionMet = EvaluateCondition(condition, dataModel, overrides);

            string selectedContent;

            if (hasElse)
            {
                var ifContent = result.Substring(condEnd + 1, elseIndex - condEnd - 1);
                var elseContent = result.Substring(elseIndex + 5, endifIndex - elseIndex - 5);
                selectedContent = conditionMet
                    ? StripControlMarkers(ifContent)
                    : StripControlMarkers(elseContent);
            }
            else
            {
                var ifContent = result.Substring(condEnd + 1, endifIndex - condEnd - 1);
                selectedContent = conditionMet ? StripControlMarkers(ifContent) : "";
            }

            result = result.Substring(0, ifIndex) + selectedContent + result.Substring(endifIndex + 6);
        }

        return result;
    }

    private static int FindMatchingEndIf(string template, int startIf)
    {
        int depth = 0;
        for (int i = startIf; i < template.Length; i++)
        {
            if (i + 3 < template.Length && template.Substring(i, 4) == "$if(") depth++;
            else if (i + 5 < template.Length && template.Substring(i, 6) == "$endif")
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
    }

    private static int FindElseIndex(string template, int start, int endIf)
    {
        int depth = 0;
        for (int i = start; i < endIf; i++)
        {
            if (i + 3 < template.Length && template.Substring(i, 4) == "$if(") depth++;
            else if (i + 5 < template.Length && template.Substring(i, 6) == "$endif") depth--;
            else if (depth == 0 && i + 4 < template.Length && template.Substring(i, 5) == "$else")
                return i;
        }
        return -1;
    }

    private static bool EvaluateCondition(string condition, object dataModel, Dictionary<string, object>? overrides = null)
    {
        if (!condition.Contains(">=") && !condition.Contains("<=") && !condition.Contains("!=") &&
            !condition.Contains("==") && !condition.Contains("<") && !condition.Contains(">"))
        {
            var val = GetPropertyValueFromChain(dataModel, condition, overrides);
            return val is bool b && b;
        }

        foreach (var op in new[] { ">=", "<=", "!=", "==", ">", "<" })
        {
            var idx = condition.IndexOf(op);
            if (idx > 0)
            {
                var left = condition.Substring(0, idx).Trim();
                var right = condition.Substring(idx + op.Length).Trim();

                var leftVal = GetPropertyValueFromChain(dataModel, left, overrides);
                var rightVal = ParseLiteral(right);

                if (leftVal == null) return false;

                switch (op)
                {
                    case ">": return CompareValues(leftVal, rightVal) > 0;
                    case "<": return CompareValues(leftVal, rightVal) < 0;
                    case ">=": return CompareValues(leftVal, rightVal) >= 0;
                    case "<=": return CompareValues(leftVal, rightVal) <= 0;
                    case "==": return Equals(leftVal, rightVal);
                    case "!=": return !Equals(leftVal, rightVal);
                }
            }
        }

        return false;
    }

    private static int CompareValues(object a, object b)
    {
        if (a is IComparable ca && b != null)
            return ca.CompareTo(b);
        return string.Compare(a?.ToString() ?? "", b?.ToString() ?? "", StringComparison.Ordinal);
    }

    private static object? GetPropertyValueFromChain(object dataModel, string chain, Dictionary<string, object>? overrides = null)
    {
        if (dataModel == null && (overrides == null || overrides.Count == 0)) return null;

        var parts = chain.Split('.');
        object? current;
        int startIndex;

        if (overrides != null && overrides.TryGetValue(parts[0], out var ov))
        {
            current = ov;
            startIndex = 1;
        }
        else
        {
            var firstProp = dataModel?.GetType().GetProperty(parts[0]);
            if (firstProp != null)
            {
                current = firstProp.GetValue(dataModel);
                startIndex = 1;
            }
            else
            {
                current = dataModel;
                startIndex = 1;
            }
        }

        for (int i = startIndex; i < parts.Length; i++)
        {
            if (current == null) return null;
            var prop = current.GetType().GetProperty(parts[i].Trim());
            if (prop == null) return null;
            current = prop.GetValue(current);
        }

        return current;
    }

    private static object ParseLiteral(string literal)
    {
        literal = literal.Trim();
        if (int.TryParse(literal, out var i)) return i;
        if (double.TryParse(literal, out var d)) return d;
        if (bool.TryParse(literal, out var bl)) return bl;
        return literal.Trim('"', '\'');
    }

    private static string StripControlMarkers(string content)
    {
        content = Regex.Replace(content, @"\$if\([^)]+\)", "");
        content = content.Replace("$else", "");
        content = content.Replace("$endif", "");
        return content;
    }

    #endregion

    #region Задача 5: Циклы $foreach / $endfor

    private static string ProcessForeachBlocks(string template, object dataModel, Dictionary<string, object>? overrides = null)
    {
        var result = template;
        int foreachIndex;

        while ((foreachIndex = result.IndexOf("$foreach(")) >= 0)
        {
            var endforIndex = FindMatchingEndFor(result, foreachIndex);
            if (endforIndex < 0) break;

            var specStart = foreachIndex + 9;
            var closeParen = result.IndexOf(')', specStart);
            if (closeParen < 0) break;
            var spec = result.Substring(specStart, closeParen - specStart).Trim();

            var inIdx = spec.IndexOf(" in ");
            if (inIdx < 0) break;
            var varName = spec.Substring(0, inIdx).Trim().Replace("var ", "");
            var collectionExpr = spec.Substring(inIdx + 4).Trim();

            var collection = GetPropertyValueFromChain(dataModel, collectionExpr, overrides) as System.Collections.IEnumerable;
            if (collection == null)
            {
                result = result.Substring(0, foreachIndex) + result.Substring(endforIndex + 7);
                continue;
            }

            var loopContent = result.Substring(closeParen + 1, endforIndex - closeParen - 1);
            var sb = new System.Text.StringBuilder();

            foreach (var item in collection)
            {
                var itemOverrides = new Dictionary<string, object> { { varName, item } };
                var processed = loopContent;
                processed = ProcessIfBlocks(processed, dataModel, itemOverrides);
                processed = ProcessForeachBlocks(processed, dataModel, itemOverrides);
                processed = SubstituteVariablesWithOverrides(processed, dataModel, itemOverrides);
                sb.Append(processed);
            }

            result = result.Substring(0, foreachIndex) + sb.ToString() + result.Substring(endforIndex + 7);
        }

        return result;
    }

    private static int FindMatchingEndFor(string template, int startForEach)
    {
        int depth = 0;
        for (int i = startForEach; i < template.Length; i++)
        {
            if (i + 8 < template.Length && template.Substring(i, 9) == "$foreach(") depth++;
            else if (i + 6 < template.Length && template.Substring(i, 7) == "$endfor")
            {
                depth--;
                if (depth == 0) return i;
            }
        }
        return -1;
    }

    private static string SubstituteVariablesWithOverrides(string template, object dataModel, Dictionary<string, object>? overrides)
    {
        if (dataModel == null && (overrides == null || overrides.Count == 0))
            return template;

        var dt = dataModel?.GetType();

        if (dt != null && (dt == typeof(string) || dt == typeof(int) || dt == typeof(double) ||
            dt == typeof(float) || dt == typeof(bool) || dt == typeof(decimal)))
        {
            return template.Replace("${dataModel}", dataModel?.ToString() ?? "");
        }

        return ReplaceVariableCore(template, dataModel, overrides);
    }

    private static string ReplaceVariableCore(string template, object? dataModel, Dictionary<string, object>? overrides)
    {
        var dt = dataModel?.GetType();
        var regex = new System.Text.RegularExpressions.Regex(@"\$\{([^}]+)\}");
        var matches = regex.Matches(template);

        if (matches.Count == 0) return template;

        var result = new System.Text.StringBuilder();
        int lastIndex = 0;

        foreach (System.Text.RegularExpressions.Match match in matches)
        {
            result.Append(template, lastIndex, match.Index - lastIndex);

            var inner = match.Groups[1].Value.Trim();
            var parts = inner.Split('.');

            object root;
            int startIndex;

            if (overrides != null && overrides.TryGetValue(parts[0], out var ov))
            {
                root = ov;
                startIndex = 1;
            }
            else
            {
                if (parts.Length == 1)
                {
                    var prop = dt?.GetProperty(parts[0]);
                    result.Append(prop != null ? prop.GetValue(dataModel)?.ToString() ?? "" : "");
                    lastIndex = match.Index + match.Length;
                    continue;
                }
                var firstProp = dt?.GetProperty(parts[0]);
                if (firstProp != null)
                {
                    root = firstProp.GetValue(dataModel)!;
                }
                else
                {
                    root = dataModel!;
                }
                startIndex = 1;
            }

            var current = root;
            string value = "";
            bool found = true;
            for (int i = startIndex; i < parts.Length; i++)
            {
                if (current == null) { value = ""; found = false; break; }
                var prop = current.GetType().GetProperty(parts[i]);
                if (prop == null) { value = ""; found = false; break; }
                current = prop.GetValue(current);
            }
            if (found && current != null) value = current.ToString() ?? "";
            result.Append(value);

            lastIndex = match.Index + match.Length;
        }

        result.Append(template, lastIndex, template.Length - lastIndex);
        return result.ToString();
    }

    #endregion
}