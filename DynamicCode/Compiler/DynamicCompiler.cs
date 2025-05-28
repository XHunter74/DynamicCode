using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace DynamicCode.Compiler;

public class DynamicCompiler
{
    private const string AssemblyName = "DynamicAssembly";

    public static Delegate CompileFunction(Type delegateType, string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var className = CompilerUtils.ExtractSingleClassName(syntaxTree);
        var methodName = CompilerUtils.ExtractMethodNameMatchingDelegate(delegateType, syntaxTree);

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create(
            assemblyName: AssemblyName,
            syntaxTrees: [syntaxTree],
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();

        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = string.Join(Environment.NewLine, result.Diagnostics.Select(d => d.ToString()));
            throw new InvalidOperationException("Compilation failed:\n" + errors);
        }

        ms.Seek(0, SeekOrigin.Begin);
        var asm = Assembly.Load(ms.ToArray());
        var type = asm.GetType(className);
        var method = type!.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

        if (method == null)
        {
            throw new MissingMethodException($"Method '{methodName}' not found in dynamic class.");
        }

        return Delegate.CreateDelegate(delegateType, method);
    }

    public static T CompileFunction<T>(string code) where T : Delegate
    {
        return (T)CompileFunction(typeof(T), code);
    }
}