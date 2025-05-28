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

var intFn = DynamicCompiler.CompileFunctionNew<Func<int, int, int>>(code);

var intResult = intFn(7, 6);

Console.WriteLine($"Function body:\r\n '{code}'");
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

var stringFn=DynamicCompiler.CompileFunctionNew<Func<string, string, string>>(code);

var stringResult = stringFn("Hello", "World");

Console.WriteLine($"Function body:\r\n '{code}'");
Console.WriteLine($"Function result is: {stringResult}");

