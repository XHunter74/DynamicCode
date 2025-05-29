namespace DynamicCode.Builder;

/// <summary>
/// Provides a fluent builder for creating delegate types (Func and Action) with specified input and output types at runtime.
/// </summary>
public class DelegateTypeBuilder
{
    private readonly List<Type> _inputs = new();
    private Type? _output;

    private DelegateTypeBuilder() { }

    /// <summary>
    /// Creates a new instance of <see cref="DelegateTypeBuilder"/>.
    /// </summary>
    /// <returns>A new <see cref="DelegateTypeBuilder"/> instance.</returns>
    public static DelegateTypeBuilder Create() => new();

    /// <summary>
    /// Adds an input parameter type to the delegate signature.
    /// </summary>
    /// <param name="inputType">The type of the input parameter.</param>
    /// <returns>The current <see cref="DelegateTypeBuilder"/> instance for chaining.</returns>
    public DelegateTypeBuilder AddInput(Type inputType)
    {
        _inputs.Add(inputType);
        return this;
    }

    /// <summary>
    /// Sets the output (return) type for the delegate signature.
    /// </summary>
    /// <param name="outputType">The type of the output (return value).</param>
    /// <returns>The current <see cref="DelegateTypeBuilder"/> instance for chaining.</returns>
    public DelegateTypeBuilder AddOutput(Type outputType)
    {
        _output = outputType;
        return this;
    }

    /// <summary>
    /// Builds a <see cref="Func"/> delegate type with the specified input and output types.
    /// </summary>
    /// <returns>The constructed <see cref="Type"/> representing the Func delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the output type is not specified.</exception>
    /// <exception cref="NotSupportedException">Thrown if the number of parameters is not supported by Func.</exception>
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

    /// <summary>
    /// Builds an <see cref="Action"/> delegate type with the specified input types.
    /// </summary>
    /// <returns>The constructed <see cref="Type"/> representing the Action delegate.</returns>
    /// <exception cref="InvalidOperationException">Thrown if an output type is specified (Action cannot have a return type).</exception>
    /// <exception cref="NotSupportedException">Thrown if the number of parameters is not supported by Action.</exception>
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