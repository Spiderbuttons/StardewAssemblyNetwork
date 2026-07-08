using FluentAssertions;
using Mono.Cecil;
using StardewAssemblyNetwork.Extensions;

namespace StardewAssemblyNetwork.Tests;

[Collection("TestContext")]
public class MethodTests
{
    private const string TEST_TYPE_NAMESPACE = "StardewAssemblyNetwork.TestAssembly";
    private const string TEST_CLASS = "MethodTests";
    private const string FULL_CLASS_NAME = $"{TEST_TYPE_NAMESPACE}.{TEST_CLASS}";

    private readonly TypeDefinition TestType;
    
    private readonly TestContext context;

    public MethodTests(TestContext ctx)
    {
        context = ctx;
        TestType = context.TestAssembly.GetType(FULL_CLASS_NAME);
    }

    private MethodDefinition GetMethod(string name)
    {
        return TestType.GetMethod(name) ?? throw new ArgumentException($"Method '{name}' not found in type '{TestType.FullName}'");
    }
    
    [Fact]
    public void NoParameters()
    {
        var method = GetMethod("GetterSetterProperty");
        // property.NormalizedName().Should().Be("GetterSetterProperty { get; set; }");
        // property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.GetterSetterProperty {{ get; set; }}");
    }
}