// Ignore Spelling: Utils

using Microsoft.CodeAnalysis.CSharp;
using DynamicCode.Compiler;

namespace DynamicCode.Test;

public class CompilerUtilsTests
{
    [Fact(DisplayName = "ExtractSingleClassName returns class name for single class")]
    public void ExtractSingleClassName_ReturnsClassName()
    {
        var code = "public class MyClass { }";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var className = CompilerUtils.ExtractSingleClassName(syntaxTree);
        Assert.Equal("MyClass", className);
    }

    [Fact(DisplayName = "ExtractSingleClassName throws if no class present")]
    public void ExtractSingleClassName_ThrowsIfNoClass()
    {
        var code = "public struct MyStruct { }";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        Assert.Throws<InvalidOperationException>(() => CompilerUtils.ExtractSingleClassName(syntaxTree));
    }

    [Fact(DisplayName = "ExtractSingleClassName throws if more than one class present")]
    public void ExtractSingleClassName_ThrowsIfMultipleClasses()
    {
        var code = "public class A { } public class B { }";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        Assert.Throws<InvalidOperationException>(() => CompilerUtils.ExtractSingleClassName(syntaxTree));
    }

    [Fact(DisplayName = "ExtractMethodNameMatchingDelegate finds method matching Func<int, int, int>")]
    public void ExtractMethodNameMatchingDelegate_FindsMatchingMethod()
    {
        var code = @"public class MyClass { public static int Calc(int x, int y) { return x + y; } }";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var methodName = CompilerUtils.ExtractMethodNameMatchingDelegate(typeof(Func<int, int, int>), syntaxTree);
        Assert.Equal("Calc", methodName);
    }

    [Fact(DisplayName = "ExtractMethodNameMatchingDelegate throws if no matching method exists")]
    public void ExtractMethodNameMatchingDelegate_ThrowsIfNoMatch()
    {
        var code = @"public class MyClass { public static int Calc(int x) { return x; } }";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        Assert.Throws<MissingMethodException>(() => CompilerUtils.ExtractMethodNameMatchingDelegate(typeof(Func<int, int, int>), syntaxTree));
    }

    [Fact(DisplayName = "ExtractMethodNameMatchingDelegate matches C# alias types to CLR types")]
    public void ExtractMethodNameMatchingDelegate_MatchesAliasTypes()
    {
        var code = @"public class MyClass { public static Int32 Calc(Int32 x, Int32 y) { return x + y; } }";
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var methodName = CompilerUtils.ExtractMethodNameMatchingDelegate(typeof(Func<int, int, int>), syntaxTree);
        Assert.Equal("Calc", methodName);
    }
}
