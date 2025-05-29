using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace DynamicCode.Compiler;

/// <summary>
/// Provides methods for compiling and executing C# code from strings at runtime using Roslyn.
/// </summary>
public class DynamicCompiler
{
    /// <summary>
    /// Compiles the provided C# code and creates a delegate of the specified type for a matching static or instance method.
    /// </summary>
    /// <param name="delegateType">The delegate type to match the method signature.</param>
    /// <param name="code">The C# code as a string containing a class and method to compile.</param>
    /// <returns>A delegate instance that can be invoked to execute the compiled method.</returns>
    /// <exception cref="InvalidOperationException">Thrown if compilation fails.</exception>
    /// <exception cref="MissingMethodException">Thrown if a matching method is not found in the compiled class.</exception>
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
            assemblyName: Constants.AssemblyName,
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

    /// <summary>
    /// Compiles the provided C# code and creates a strongly-typed delegate for a matching static or instance method.
    /// </summary>
    /// <typeparam name="T">The delegate type to match the method signature.</typeparam>
    /// <param name="code">The C# code as a string containing a class and method to compile.</param>
    /// <returns>A strongly-typed delegate instance that can be invoked to execute the compiled method.</returns>
    /// <exception cref="InvalidOperationException">Thrown if compilation fails.</exception>
    /// <exception cref="MissingMethodException">Thrown if a matching method is not found in the compiled class.</exception>
    public static T CompileFunction<T>(string code) where T : Delegate
    {
        return (T)CompileFunction(typeof(T), code);
    }
}