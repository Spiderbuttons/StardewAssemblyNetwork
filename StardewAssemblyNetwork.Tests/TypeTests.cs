using StardewAssemblyNetwork.Extensions;

[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace StardewAssemblyNetwork.Tests;

[Collection("TestContext")]
public class TypeTests
{
    private readonly TestContext context;

    public TypeTests(TestContext ctx)
    {
        context = ctx;
    }
    
    [Fact]
    public void NormalType()
    {
        var type = context.TestAssembly.GetType("StardewAssemblyNetwork.TestAssembly.NormalType");
        Assert.Equal("NormalType", type.NormalizedName());
        Assert.Equal("StardewAssemblyNetwork.TestAssembly.NormalType", type.NormalizedFullName());
    }

    [Fact]
    public void SingleGenericType()
    {
        var type = context.TestAssembly.GetType("StardewAssemblyNetwork.TestAssembly.SingleGenericType`1");
        Assert.Equal("SingleGenericType<T>", type.NormalizedName());
        Assert.Equal("StardewAssemblyNetwork.TestAssembly.SingleGenericType<T>", type.NormalizedFullName());
    }

    [Fact]
    public void DoubleGenericType()
    {
        var type = context.TestAssembly.GetType("StardewAssemblyNetwork.TestAssembly.DoubleGenericType`2");
        Assert.Equal("DoubleGenericType<T1, T2>", type.NormalizedName());
        Assert.Equal("StardewAssemblyNetwork.TestAssembly.DoubleGenericType<T1, T2>", type.NormalizedFullName());
    }

    [Fact]
    public void NamedSingleGeneric()
    {
        var type = context.TestAssembly.GetType("StardewAssemblyNetwork.TestAssembly.NamedSingleGenericType`1");
        Assert.Equal("NamedSingleGenericType<SomeType>", type.NormalizedName());
        Assert.Equal("StardewAssemblyNetwork.TestAssembly.NamedSingleGenericType<SomeType>", type.NormalizedFullName());
    }

    [Fact]
    public void NamedDoubleGeneric()
    {
        var type = context.TestAssembly.GetType("StardewAssemblyNetwork.TestAssembly.NamedDoubleGenericType`2");
        Assert.Equal("NamedDoubleGenericType<Type1, Type2>", type.NormalizedName());
        Assert.Equal("StardewAssemblyNetwork.TestAssembly.NamedDoubleGenericType<Type1, Type2>", type.NormalizedFullName());
    }

    [Fact]
    public void NestedType()
    {
        var type = context.TestAssembly.GetType("StardewAssemblyNetwork.TestAssembly.NormalType/NestedType");
        Assert.Equal("NestedType", type.NormalizedName());
        Assert.Equal("StardewAssemblyNetwork.TestAssembly.NormalType+NestedType", type.NormalizedFullName());
    }

    [Fact]
    public void DualNestedType()
    {
        var type = context.TestAssembly.GetType("StardewAssemblyNetwork.TestAssembly.NormalType/NestedType/DualNestedType");
        Assert.Equal("DualNestedType", type.NormalizedName());
        Assert.Equal("StardewAssemblyNetwork.TestAssembly.NormalType+NestedType+DualNestedType", type.NormalizedFullName());
    }
}