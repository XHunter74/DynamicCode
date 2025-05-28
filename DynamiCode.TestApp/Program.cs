using DynamicCode.Builder;
using DynamicCode.Compiler;

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
Console.WriteLine("x = 7, y = 6");
Console.WriteLine($"Function result is: {intResult}");

var intFn1 = DynamicCompiler.CompileFunction<Func<int, int, int>>(code);

intResult = intFn1(8, 9);

Console.WriteLine($"Function body:\r\n'{code}'");
Console.WriteLine("x = 8, y = 9");
Console.WriteLine($"Function result is: {intResult}");

code = @"
        using System;
        public static class DynamicClass
          {
              public static string Calculate(string x, string y)
              {
                  return x + "" "" + y;
              }
          }
";

var stringFuncType = DelegateTypeBuilder.Create()
    .AddInput(typeof(string))
    .AddInput(typeof(string))
    .AddOutput(typeof(string))
    .BuildFuncType();

var stringFn = DynamicCompiler.CompileFunction(stringFuncType, code);

var stringResult = (string)stringFn.DynamicInvoke("Hello", "World");

Console.WriteLine($"Function body:\r\n'{code}'");
Console.WriteLine("x = 'Hello', y = 'World'");
Console.WriteLine($"Function result is: {stringResult}");

