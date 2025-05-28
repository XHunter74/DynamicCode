using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Reflection;

namespace DynamicCode;

public class DynamicCompiler
{
    public static T CompileFunctionNew<T>(string code) where T : Delegate
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var className = ExtractSingleClassName(syntaxTree);
        var methodName = ExtractMethodNameMatchingDelegate<T>(syntaxTree);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create(
            assemblyName: "DynamicAssembly",
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();

        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString()));
            throw new Exception("Compilation failed:\n" + errors);
        }

        ms.Seek(0, SeekOrigin.Begin);
        var asm = Assembly.Load(ms.ToArray());
        var type = asm.GetType(className);
        var method = type!.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

        if (method == null)
        {
            throw new Exception($"Method '{methodName}' not found in dynamic class.");
        }

        return (T)method.CreateDelegate(typeof(T), null);
    }

    private static string ExtractSingleClassName(SyntaxTree syntaxTree)
    {
        var root = syntaxTree.GetRoot();
        var classDecls = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
        if (classDecls.Count == 0)
            throw new Exception("No class declaration found in code.");
        if (classDecls.Count > 1)
            throw new Exception("More than one class declaration found in code.");
        return classDecls[0].Identifier.Text;
    }

    private static string ExtractMethodNameMatchingDelegate<T>(SyntaxTree syntaxTree) where T : Delegate
    {
        var delegateInvoke = typeof(T).GetMethod("Invoke");
        var delegateParams = delegateInvoke.GetParameters();
        var delegateReturn = delegateInvoke.ReturnType;
        var root = syntaxTree.GetRoot();
        var methodDecls = root.DescendantNodes().OfType<MethodDeclarationSyntax>();

        foreach (var method in methodDecls)
        {
            // Compare return type
            var methodReturnType = method.ReturnType.ToString();
            var clrReturnType = CompilerUtils.GetClrTypeName(methodReturnType);
            if (clrReturnType != delegateReturn.Name && clrReturnType != delegateReturn.FullName)
                continue;
            var parameters = method.ParameterList.Parameters;
            if (parameters.Count != delegateParams.Length)
                continue;
            bool match = true;
            for (int i = 0; i < parameters.Count; i++)
            {
                var paramType = parameters[i].Type?.ToString();
                var clrParamType = CompilerUtils.GetClrTypeName(paramType);
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
        throw new Exception($"No method matching delegate {typeof(T).Name} found in syntax tree.");
    }
}