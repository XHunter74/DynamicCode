using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DynamicCode.Compiler;

internal class CompilerUtils
{
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

    public static string ExtractMethodNameMatchingDelegate(Type delegateType,SyntaxTree syntaxTree)
    {
        var delegateInvoke = delegateType.GetMethod("Invoke");
        var delegateParams = delegateInvoke.GetParameters();
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
        throw new InvalidOperationException($"No method matching delegate {delegateType.Name} found in syntax tree.");
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

    private static string GetClrTypeName(string csharpType)
    {
        if (CSharpToClrTypeMap.TryGetValue(csharpType, out var clrName))
            return clrName;
        return csharpType;
    }
}
