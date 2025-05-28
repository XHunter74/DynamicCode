using DynamicCode;

var newBody = "return x * y;";

var fn = DynamicCompiler.CompileFunction(newBody);

var result = fn(7, 6);

Console.WriteLine($"Function body: {newBody}");
Console.WriteLine($"Function result is: {result}");

newBody = "return x * y - 5;";
fn = DynamicCompiler.CompileFunction(newBody);

result = fn(7, 6);

Console.WriteLine($"Function body: {newBody}");
Console.WriteLine($"Function result is: {result}");