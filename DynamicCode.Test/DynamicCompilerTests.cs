using System;
using Xunit;
using DynamicCode.Compiler;

namespace DynamicCode.Test;

public class DynamicCompilerTests
{
    [Fact(DisplayName = "CompileFunction<T> compiles and invokes Func<int, int, int> correctly")]
    public void CompileFunction_GenericFuncIntIntInt_Works()
    {
        var code = @"public static class DynamicClass { public static int Calculate(int x, int y) { return x + y; } }";
        var fn = DynamicCompiler.CompileFunction<Func<int, int, int>>(code);
        Assert.Equal(12, fn(7, 5));
    }

    [Fact(DisplayName = "CompileFunction(Type, code) compiles and invokes Func<string, string, string> correctly")]
    public void CompileFunction_TypeFuncStringStringString_Works()
    {
        var code = @"public static class DynamicClass { public static string Calculate(string a, string b) { return a + b; } }";
        var funcType = typeof(Func<string, string, string>);
        var fn = DynamicCompiler.CompileFunction(funcType, code);
        var result = fn.DynamicInvoke("Hello", "World");
        Assert.Equal("HelloWorld", result);
    }

    [Fact(DisplayName = "CompileFunction throws on compilation error")]
    public void CompileFunction_ThrowsOnCompilationError()
    {
        var code = @"public static class DynamicClass { public static int Calculate(int x, int y) { return x + ; } }";
        Assert.Throws<InvalidOperationException>(() => DynamicCompiler.CompileFunction<Func<int, int, int>>(code));
    }

    [Fact(DisplayName = "CompileFunction throws if method not found")]
    public void CompileFunction_ThrowsIfMethodNotFound()
    {
        var code = @"public static class DynamicClass { public static int NotCalculate(int x, int y) { return x + y; } }";
        Assert.Throws<MissingMethodException>(() => DynamicCompiler.CompileFunction<Func<int, int, long>>(code));
    }

    [Fact(DisplayName = "CompileFunction works with C# alias and CLR types mixed")]
    public void CompileFunction_WorksWithAliasAndClrTypes()
    {
        var code = @"using System; public static class DynamicClass { public static Int32 Calculate(int x, Int32 y) { return x + y; } }";
        var fn = DynamicCompiler.CompileFunction<Func<int, int, int>>(code);
        Assert.Equal(11, fn(5, 6));
    }
}
