using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace DynamicCode;

public class DynamicCompiler
{
    public static T CompileFunctionNew<T>(string code) where T : Delegate
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var className = CompilerUtils.ExtractSingleClassName(syntaxTree);
        var methodName = CompilerUtils.ExtractMethodNameMatchingDelegate<T>(syntaxTree);

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

        return (T)method.CreateDelegate(typeof(T), null);
    }

}