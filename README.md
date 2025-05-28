# DynamicCodeFromString

A .NET 8 project for compiling and executing C# code from strings at runtime using Roslyn. This project demonstrates how to dynamically compile code, extract class and method information, and create delegates for runtime execution.

## Features
- Compile C# code from a string at runtime
- Extract class and method names from code using Roslyn syntax trees
- Match methods to delegate signatures
- Create delegates for dynamically compiled methods
- Example usage for both integer and string functions

## Requirements
- .NET 8 SDK
- NuGet package: `Microsoft.CodeAnalysis.CSharp`

## Usage Example

```
csharp
using DynamicCode;

var code = @"
    using System;
    public static class DynamicClass
    {
        public static int Calculate(int x, int y)
        {
            return x * y;
        }
    }
";

var intFn = DynamicCompiler.CompileFunctionNew<Func<int, int, int>>(code);
var intResult = intFn(7, 6);
Console.WriteLine($"Function result is: {intResult}");

code = @"
    using System;
    public static class DynamicClass
    {
        public static string Calculate(string x, string y)
        {
            return x + \" \" + y;
        }
    }
";

var stringFn = DynamicCompiler.CompileFunctionNew<Func<string, string, string>>(code);
var stringResult = stringFn("Hello", "World");
Console.WriteLine($"Function result is: {stringResult}");
```

## Project Structure
- `DynamicCompiler`: Main API for compiling code and creating delegates
- `CompilerUtils`: Utilities for extracting class and method names from syntax trees
- `SimpleDynamicCompiler`: Minimal example for compiling a specific function signature
- `Program.cs`: Example usage

## License
MIT License
Copyright (c) 2025 Serhiy Krasovskyy xhunter74@gmail.com