using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace DynamicCode.Compiler;

public class SimpleDynamicCompiler
{
    public static Func<int, int, int> CompileFunction(string body)
    {
        // Source code for a class with a static method Calculate
        string code = $@"
            using System;
            public static class DynamicClass
            {{
                public static int Calculate(int x, int y)
                {{
                    {body}
                }}
            }}";

        var syntaxTree = CSharpSyntaxTree.ParseText(code);
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
        var type = asm.GetType("DynamicClass");
        var method = type!.GetMethod("Calculate", BindingFlags.Public | BindingFlags.Static);

        return (Func<int, int, int>)method!.CreateDelegate(typeof(Func<int, int, int>));
    }
}
