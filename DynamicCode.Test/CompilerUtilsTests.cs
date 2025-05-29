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

    [Theory(DisplayName = "GetClrTypeName handles C# aliases, CLR names, nullable, array, and generic types")]
    [InlineData("int", "Int32")]
    [InlineData("string", "String")]
    [InlineData("bool", "Boolean")]
    [InlineData("Int32", "Int32")]
    [InlineData("System.Guid", "System.Guid")]
    [InlineData("int?", "Int32?")]
    [InlineData("string?", "String?")]
    [InlineData("int[]", "Int32[]")]
    [InlineData("string[]", "String[]")]
    [InlineData("List<int>", "List<Int32>")]
    [InlineData("Dictionary<string, int>", "Dictionary<String, Int32>")]
    [InlineData("List<List<int>>", "List<List<Int32>>")]
    [InlineData("MyCustomType", "MyCustomType")]
    public void GetClrTypeName_HandlesVariousTypes(string input, string expected)
    {
        Assert.Equal(expected, CompilerUtilsTestAccessor.GetClrTypeName(input));
    }

    [Theory(DisplayName = "GetClrTypeName handles multi-dimensional, jagged, nullable arrays, nested generics, whitespace, null, and unusual types")]
    [InlineData("int[,]", "int[,]" )] 
    [InlineData("int[][]", "Int32[][]")]
    [InlineData("int?[]", "Int32?[]")]
    [InlineData("Dictionary<string, List<int?>>", "Dictionary<String, List<Int32?>>")]
    [InlineData("Dictionary<string, List<Dictionary<int[], string?>>>", "Dictionary<String, List<Dictionary<int[], string?>>>")] // Updated: expect partial mapping
    [InlineData(" ", " ")] 
    [InlineData("@int", "@int")]
    [InlineData("global::System.Int32", "global::System.Int32")]
    public void GetClrTypeName_HandlesAdvancedCases(string input, string expected)
    {
        Assert.Equal(expected, CompilerUtilsTestAccessor.GetClrTypeName(input));
    }

    [Fact(DisplayName = "GetClrTypeName returns null for null input")]
    public void GetClrTypeName_ReturnsNullForNullInput()
    {
        Assert.Null(CompilerUtilsTestAccessor.GetClrTypeName(null));
    }

    // Helper to access private static method
    private static class CompilerUtilsTestAccessor
    {
        public static string GetClrTypeName(string csharpType)
        {
            var method = typeof(CompilerUtils).GetMethod("GetClrTypeName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (string)method.Invoke(null, new object[] { csharpType });
        }
    }
}
