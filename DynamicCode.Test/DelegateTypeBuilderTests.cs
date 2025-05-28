using DynamicCode.Builder;

namespace DynamicCode.Test;

public class DelegateTypeBuilderTests
{
    [Fact]
    public void BuildFuncType_WithOneInput_ReturnsFuncType()
    {
        var type = DelegateTypeBuilder.Create()
            .AddInput(typeof(int))
            .AddOutput(typeof(string))
            .BuildFuncType();
        Assert.Equal(typeof(Func<int, string>), type);
    }

    [Fact]
    public void BuildFuncType_WithTwoInputs_ReturnsFuncType()
    {
        var type = DelegateTypeBuilder.Create()
            .AddInput(typeof(int))
            .AddInput(typeof(int))
            .AddOutput(typeof(int))
            .BuildFuncType();
        Assert.Equal(typeof(Func<int, int, int>), type);
    }

    [Fact]
    public void BuildFuncType_WithoutOutput_Throws()
    {
        var builder = DelegateTypeBuilder.Create().AddInput(typeof(int));
        Assert.Throws<InvalidOperationException>(() => builder.BuildFuncType());
    }

    [Fact]
    public void BuildFuncType_TooManyInputs_Throws()
    {
        var builder = DelegateTypeBuilder.Create();
        for (int i = 0; i < 8; i++) builder.AddInput(typeof(int));
        builder.AddOutput(typeof(int));
        Assert.Throws<NotSupportedException>(() => builder.BuildFuncType());
    }

    [Fact]
    public void BuildActionType_WithInputs_ReturnsActionType()
    {
        var type = DelegateTypeBuilder.Create()
            .AddInput(typeof(int))
            .AddInput(typeof(string))
            .BuildActionType();
        Assert.Equal(typeof(Action<int, string>), type);
    }

    [Fact]
    public void BuildActionType_WithOutput_Throws()
    {
        var builder = DelegateTypeBuilder.Create()
            .AddInput(typeof(int))
            .AddOutput(typeof(int));
        Assert.Throws<InvalidOperationException>(() => builder.BuildActionType());
    }

    [Fact]
    public void BuildActionType_TooManyInputs_Throws()
    {
        var builder = DelegateTypeBuilder.Create();
        for (int i = 0; i < 9; i++) builder.AddInput(typeof(int));
        Assert.Throws<NotSupportedException>(() => builder.BuildActionType());
    }
}
