namespace DynamicCode.Compiler;

public class DelegateTypeBuilder
{
    private readonly List<Type> _inputs = new();
    private Type? _output;

    private DelegateTypeBuilder() { }

    public static DelegateTypeBuilder Create() => new();

    public DelegateTypeBuilder AddInput(Type inputType)
    {
        _inputs.Add(inputType);
        return this;
    }

    public DelegateTypeBuilder AddOutput(Type outputType)
    {
        _output = outputType;
        return this;
    }

    public Type BuildFuncType()
    {
        if (_output == null)
            throw new InvalidOperationException("Output type must be specified for Func.");
        var typeArgs = new List<Type>(_inputs) { _output };
        return typeArgs.Count switch
        {
            2 => typeof(Func<,>).MakeGenericType(typeArgs.ToArray()),
            3 => typeof(Func<,,>).MakeGenericType(typeArgs.ToArray()),
            4 => typeof(Func<,,,>).MakeGenericType(typeArgs.ToArray()),
            5 => typeof(Func<,,,,>).MakeGenericType(typeArgs.ToArray()),
            6 => typeof(Func<,,,,,>).MakeGenericType(typeArgs.ToArray()),
            7 => typeof(Func<,,,,,,>).MakeGenericType(typeArgs.ToArray()),
            8 => typeof(Func<,,,,,,,>).MakeGenericType(typeArgs.ToArray()),
            _ => throw new NotSupportedException($"Func with {typeArgs.Count - 1} parameters is not supported.")
        };
    }

    public Type BuildActionType()
    {
        if (_output != null)
            throw new InvalidOperationException("Action cannot have an output type.");
        return _inputs.Count switch
        {
            1 => typeof(Action<>).MakeGenericType(_inputs.ToArray()),
            2 => typeof(Action<,>).MakeGenericType(_inputs.ToArray()),
            3 => typeof(Action<,,>).MakeGenericType(_inputs.ToArray()),
            4 => typeof(Action<,,,>).MakeGenericType(_inputs.ToArray()),
            5 => typeof(Action<,,,,>).MakeGenericType(_inputs.ToArray()),
            6 => typeof(Action<,,,,,>).MakeGenericType(_inputs.ToArray()),
            7 => typeof(Action<,,,,,,>).MakeGenericType(_inputs.ToArray()),
            8 => typeof(Action<,,,,,,,>).MakeGenericType(_inputs.ToArray()),
            _ => throw new NotSupportedException($"Action with {_inputs.Count} parameters is not supported.")
        };
    }
}