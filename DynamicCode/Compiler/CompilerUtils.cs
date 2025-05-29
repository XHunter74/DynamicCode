// Ignore Spelling: Utils

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DynamicCode.Compiler;

/// <summary>
/// Utility methods for extracting class and method names from Roslyn syntax trees and mapping C# type names to CLR type names.
/// </summary>
public class CompilerUtils
{
    /// <summary>
    /// Extracts the name of the single class declared in the provided syntax tree.
    /// Throws an exception if there are zero or more than one class declarations.
    /// </summary>
    /// <param name="syntaxTree">The Roslyn syntax tree to analyze.</param>
    /// <returns>The name of the single class declared in the syntax tree.</returns>
    /// <exception cref="InvalidOperationException">Thrown if zero or more than one class declarations are found.</exception>
    public static string ExtractSingleClassName(SyntaxTree syntaxTree)
    {
        var root = syntaxTree.GetRoot();
        var classDecls = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        if (classDecls.Count == 0)
            throw new InvalidOperationException("No class declaration found in code.");
        if (classDecls.Count > 1)
            throw new InvalidOperationException("More than one class declaration found in code.");
        return classDecls[0].Identifier.Text;
    }

    /// <summary>
    /// Extracts the name of the method in the syntax tree that matches the signature of the provided delegate type.
    /// </summary>
    /// <param name="delegateType">The delegate type to match against.</param>
    /// <param name="syntaxTree">The Roslyn syntax tree to analyze.</param>
    /// <returns>The name of the matching method.</returns>
    /// <exception cref="MissingMethodException">Thrown if no matching method is found.</exception>
    public static string ExtractMethodNameMatchingDelegate(Type delegateType, SyntaxTree syntaxTree)
    {
        var delegateInvoke = delegateType.GetMethod(Constants.InvokeMethodName);
        var delegateParams = delegateInvoke!.GetParameters();
        var delegateReturn = delegateInvoke.ReturnType;
        var root = syntaxTree.GetRoot();
        var methodDecls = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        foreach (var method in methodDecls)
        {
            var methodReturnType = method.ReturnType.ToString();
            var clrReturnType = GetClrTypeName(methodReturnType);
            if (clrReturnType != delegateReturn.Name && clrReturnType != delegateReturn.FullName)
                continue;
            var parameters = method.ParameterList.Parameters;
            if (parameters.Count != delegateParams.Length)
                continue;
            bool match = true;
            for (int i = 0; i < parameters.Count; i++)
            {
                var paramType = parameters[i].Type?.ToString();
                if (paramType == null)
                {
                    match = false;
                    break;
                }
                var clrParamType = GetClrTypeName(paramType);
                var delegateParamType = delegateParams[i].ParameterType;
                if (clrParamType != delegateParamType.Name && clrParamType != delegateParamType.FullName)
                {
                    match = false;
                    break;
                }
            }
            if (match)
                return method.Identifier.Text;
        }
        throw new MissingMethodException($"No method matching delegate {delegateType.Name} found in syntax tree.");
    }

    private static readonly Dictionary<string, string> CSharpToClrTypeMap = new()
    {
        {"bool", "Boolean"},
        {"byte", "Byte"},
        {"sbyte", "SByte"},
        {"char", "Char"},
        {"decimal", "Decimal"},
        {"double", "Double"},
        {"float", "Single"},
        {"int", "Int32"},
        {"uint", "UInt32"},
        {"long", "Int64"},
        {"ulong", "UInt64"},
        {"object", "Object"},
        {"short", "Int16"},
        {"ushort", "UInt16"},
        {"string", "String"}
    };

    /// <summary>
    /// Maps a C# type name or keyword to its CLR type name. Handles nullable types (e.g., int?), array types (e.g., int[]), and generic types (e.g., List&lt;int&gt;).
    /// Falls back to the input if no mapping is found.
    /// </summary>
    /// <param name="csharpType">The C# type name as a string.</param>
    /// <returns>The CLR type name as a string.</returns>
    private static string GetClrTypeName(string csharpType)
    {
        if (string.IsNullOrWhiteSpace(csharpType))
            return csharpType;

        // Handle nullable types (e.g., int?)
        if (csharpType.EndsWith('?'))
        {
            var baseType = csharpType.TrimEnd('?');
            var clrBase = GetClrTypeName(baseType);
            return clrBase + "?";
        }

        // Handle array types (e.g., int[])
        if (csharpType.EndsWith("[]"))
        {
            var baseType = csharpType.Substring(0, csharpType.Length - 2);
            var clrBase = GetClrTypeName(baseType);
            return clrBase + "[]";
        }

        // Handle generic types (e.g., List<int>)
        var genericTick = csharpType.IndexOf('<');
        if (genericTick > 0 && csharpType.EndsWith(">"))
        {
            var mainType = csharpType.Substring(0, genericTick);
            var genericArgs = csharpType.Substring(genericTick + 1, csharpType.Length - genericTick - 2);
            var clrMain = GetClrTypeName(mainType);
            var clrArgs = string.Join(", ", genericArgs.Split(',').Select(arg => GetClrTypeName(arg.Trim())));
            return $"{clrMain}<{clrArgs}>";
        }

        if (CSharpToClrTypeMap.TryGetValue(csharpType, out var clrName))
            return clrName;
        return csharpType;
    }
}
