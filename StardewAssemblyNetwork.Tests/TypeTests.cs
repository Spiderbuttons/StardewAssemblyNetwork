using Mono.Cecil;
using StardewAssemblyNetwork.Extensions;

namespace StardewAssemblyNetwork.Tests;

[Collection("TestContext")]
public class TypeTests
{
    private const string TEST_TYPE_NAMESPACE = "StardewAssemblyNetwork.TestAssembly";
    
    private readonly TestContext context;

    public TypeTests(TestContext ctx)
    {
        context = ctx;
    }

    private TypeDefinition GetType(string name)
    {
        return context.TestAssembly.GetType($"{TEST_TYPE_NAMESPACE}.{name}");
    }
    
    [Fact]
    public void NormalType()
    {
        var type = GetType("NormalType");
        Assert.Equal("NormalType", type.NormalizedName());
        Assert.Equal($"{TEST_TYPE_NAMESPACE}.NormalType", type.NormalizedFullName());
    }

    [Fact]
    public void SingleGenericType()
    {
        var type = GetType("SingleGenericType`1");
        Assert.Equal("SingleGenericType<T>", type.NormalizedName());
        Assert.Equal($"{TEST_TYPE_NAMESPACE}.SingleGenericType<T>", type.NormalizedFullName());
    }

    [Fact]
    public void DoubleGenericType()
    {
        var type = GetType("DoubleGenericType`2");
        Assert.Equal("DoubleGenericType<T1, T2>", type.NormalizedName());
        Assert.Equal($"{TEST_TYPE_NAMESPACE}.DoubleGenericType<T1, T2>", type.NormalizedFullName());
    }

    [Fact]
    public void NamedSingleGeneric()
    {
        var type = GetType("NamedSingleGenericType`1");
        Assert.Equal("NamedSingleGenericType<SomeType>", type.NormalizedName());
        Assert.Equal($"{TEST_TYPE_NAMESPACE}.NamedSingleGenericType<SomeType>", type.NormalizedFullName());
    }

    [Fact]
    public void NamedDoubleGeneric()
    {
        var type = GetType("NamedDoubleGenericType`2");
        Assert.Equal("NamedDoubleGenericType<Type1, Type2>", type.NormalizedName());
        Assert.Equal($"{TEST_TYPE_NAMESPACE}.NamedDoubleGenericType<Type1, Type2>", type.NormalizedFullName());
    }

    [Fact]
    public void NestedType()
    {
        var type = GetType("NormalType/NestedType");
        Assert.Equal("NestedType", type.NormalizedName());
        Assert.Equal($"{TEST_TYPE_NAMESPACE}.NormalType+NestedType", type.NormalizedFullName());
    }

    [Fact]
    public void DualNestedType()
    {
        var type = GetType("NormalType/NestedType/DualNestedType");
        Assert.Equal("DualNestedType", type.NormalizedName());
        Assert.Equal($"{TEST_TYPE_NAMESPACE}.NormalType+NestedType+DualNestedType", type.NormalizedFullName());
    }
}