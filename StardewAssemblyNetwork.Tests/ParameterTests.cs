using System.Reflection;
using FluentAssertions;
using Mono.Cecil;
using StardewAssemblyNetwork.Extensions;

namespace StardewAssemblyNetwork.Tests;

[Collection("TestContext")]
public class ParameterTests
{
    private const string TEST_TYPE_NAMESPACE = "StardewAssemblyNetwork.TestAssembly";
    private const string TEST_CLASS = "ParameterTests";
    private const string FULL_CLASS_NAME = $"{TEST_TYPE_NAMESPACE}.{TEST_CLASS}";

    private readonly TypeDefinition TestType;
    
    private readonly TestContext context;

    public ParameterTests(TestContext ctx)
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
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        method.GetParameters().Should().BeEmpty();
    }
    
    [Fact]
    public void OneParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("int index");
    }
    
    [Fact]
    public void TwoParameters()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(2).And
            .Contain(["int index1", "int index2"]);
    }
    
    [Fact]
    public void ReferenceParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.ParameterTests parameter");
    }
    
    [Fact]
    public void StructParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.ParameterTests+StructType structType");
    }
    
    [Fact]
    public void InParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("in int index");
    }
    
    [Fact]
    public void OutParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("out int index");
    }
    
    [Fact]
    public void RefParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("ref int index");
    }
    
    [Fact]
    public void InOutParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("in out int index");
    }
    
    [Fact]
    public void RefReadonlyParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("ref readonly int index");
    }
    
    [Fact]
    public void OptionalParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("int index = 1");
    }
    
    [Fact]
    public void ObjectParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("object index");
    }
    
    [Fact]
    public void DynamicParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var firstParam = method.GetParameters()[0];
        foreach (var attr in firstParam.CustomAttributes)
        {
            Console.WriteLine(attr.AttributeType.Name);
        }
        
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("dynamic index");
    }
}