# DynamicCodeFromString

A .NET 8 project for compiling and executing C# code from strings at runtime using Roslyn. This project demonstrates how to dynamically compile code, extract class and method information, and create delegates for runtime execution.

## Features
- Compile C# code from a string at runtime
- Extract class and method names from code using Roslyn syntax trees
- Match methods to delegate signatures
- Create delegates for dynamically compiled methods
- Example usage for both integer and string functions
- Comprehensive test suite
- Published as a NuGet package: `dynamic-code`

## Requirements
- .NET 8 SDK
- NuGet package: `dynamic-code` ([NuGet Gallery](https://www.nuget.org/packages/dynamic-code))

## Installation

Install the package from NuGet:
dotnet add package dynamic-code

## Usage Example
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

var intFuncType = DelegateTypeBuilder.Create()
    .AddInput(typeof(int))
    .AddInput(typeof(int))
    .AddOutput(typeof(int))
    .BuildFuncType();

var intFn = DynamicCompiler.CompileFunction(intFuncType, code);

var intResult = (int)intFn.DynamicInvoke(7, 6);

Console.WriteLine($"Function body:\r\n'{code}'");
Console.WriteLine($"Function result is: {intResult}");

// The same thing, but in a different way

var intFn1 = DynamicCompiler.CompileFunction<Func<int, int, int>>(code);

intResult = intFn1(8, 9);

Console.WriteLine($"Function body:\r\n'{code}'");
Console.WriteLine("x = 8, y = 9");
Console.WriteLine($"Function result is: {intResult}");

## Project Structure

- `DynamicCode/Compiler/` - Core compiler and utility classes
    - `DynamicCompiler`: Main API for compiling code and creating delegates
    - `CompilerUtils`: Utilities for extracting class and method names from syntax trees
    - `DelegateTypeBuilder`: Fluent builder for delegate types
    - `SimpleDynamicCompiler`: Minimal example for compiling a specific function signature
- `DynamicCode.Test/` - Unit tests for all major features
- `Program.cs`: Example usage

## Running Tests

To run the test suite:
dotnet test

## License
MIT License

Copyright (c) 2025 Serhiy Krasovskyy xhunter74@gmail.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
