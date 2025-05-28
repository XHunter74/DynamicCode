using DynamicCode.Builder;

namespace DynamicCode.Test;

public class DelegateTypeBuilderTests
{
    [Fact(DisplayName = "BuildFuncType returns Func<int, string> for one input and one output")]
    public void BuildFuncType_WithOneInput_ReturnsFuncType()
    {
        var type = DelegateTypeBuilder.Create()
            .AddInput(typeof(int))
            .AddOutput(typeof(string))
            .BuildFuncType();
        Assert.Equal(typeof(Func<int, string>), type);
    }

    [Fact(DisplayName = "BuildFuncType returns Func<int, int, int> for two inputs and one output")]
    public void BuildFuncType_WithTwoInputs_ReturnsFuncType()
    {
        var type = DelegateTypeBuilder.Create()
            .AddInput(typeof(int))
            .AddInput(typeof(int))
            .AddOutput(typeof(int))
            .BuildFuncType();
        Assert.Equal(typeof(Func<int, int, int>), type);
    }

    [Fact(DisplayName = "BuildFuncType throws InvalidOperationException if output is not specified")]
    public void BuildFuncType_WithoutOutput_Throws()
    {
        var builder = DelegateTypeBuilder.Create().AddInput(typeof(int));
        Assert.Throws<InvalidOperationException>(() => builder.BuildFuncType());
    }

    [Fact(DisplayName = "BuildFuncType throws NotSupportedException if too many inputs are specified")]
    public void BuildFuncType_TooManyInputs_Throws()
    {
        var builder = DelegateTypeBuilder.Create();
        for (int i = 0; i < 8; i++) builder.AddInput(typeof(int));
        builder.AddOutput(typeof(int));
        Assert.Throws<NotSupportedException>(() => builder.BuildFuncType());
    }

    [Fact(DisplayName = "BuildActionType returns Action<int, string> for two inputs and no output")]
    public void BuildActionType_WithInputs_ReturnsActionType()
    {
        var type = DelegateTypeBuilder.Create()
            .AddInput(typeof(int))
            .AddInput(typeof(string))
            .BuildActionType();
        Assert.Equal(typeof(Action<int, string>), type);
    }

    [Fact(DisplayName = "BuildActionType throws InvalidOperationException if output is specified")]
    public void BuildActionType_WithOutput_Throws()
    {
        var builder = DelegateTypeBuilder.Create()
            .AddInput(typeof(int))
            .AddOutput(typeof(int));
        Assert.Throws<InvalidOperationException>(() => builder.BuildActionType());
    }

    [Fact(DisplayName = "BuildActionType throws NotSupportedException if too many inputs are specified")]
    public void BuildActionType_TooManyInputs_Throws()
    {
        var builder = DelegateTypeBuilder.Create();
        for (int i = 0; i < 9; i++) builder.AddInput(typeof(int));
        Assert.Throws<NotSupportedException>(() => builder.BuildActionType());
    }
}
