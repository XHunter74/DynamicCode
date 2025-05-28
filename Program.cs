using DynamicCode;

var newBody = "return x * y;";

var fn = DynamicCompiler.CompileFunctionNew<Func<int, int, int>>(newBody);

var result = fn(7, 6);

Console.WriteLine($"Function body: {newBody}");
Console.WriteLine($"Function result is: {result}");

newBody = "return x * y - 5;";
fn = SimpleDynamicCompiler.CompileFunction(newBody);

result = fn(7, 6);

Console.WriteLine($"Function body: {newBody}");
Console.WriteLine($"Function result is: {result}");