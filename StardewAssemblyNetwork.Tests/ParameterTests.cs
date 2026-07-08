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
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("int index");
    }
    
    [Fact]
    public void TwoParameters()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
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
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("dynamic index");
    }
    
    [Fact]
    public void NullableValueTypeParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("int? index");
    }
    
    [Fact]
    public void NullableReferenceTypeParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName(method)).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.ParameterTests? parameter");
    }
    
    [Fact]
    public void NullableStructTypeParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.ParameterTests+StructType? parameter");
    }
    
    [Fact]
    public void AllNullableParameters()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(3).And
            .Contain(["int? index1", "int? index2", "int? index3"]);
    }
    
    [Fact]
    public void MostlyNullableParameters()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(3).And
            .Contain(["int? index1", "int? index2", "int index3"]);
    }
    
    [Fact]
    public void MostlyNonNullableParameters()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(3).And
            .Contain(["int? index1", "int index2", "int index3"]);
    }
    
    [Fact]
    public void ValueTypeArrayParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("int[] indexes");
    }
    
    [Fact]
    public void ReferenceTypeArrayParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("string[] strings");
    }
    
    [Fact]
    public void NullableArrayParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("string[]? strings");
    }
    
    [Fact]
    public void NullableNullableArrayParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("string?[]? strings");
    }
    
    [Fact]
    public void SingleGenericParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.SingleGenericType<int> singleGeneric");
    }
    
    [Fact]
    public void DoubleGenericParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.DoubleGenericType<int, string> doubleGeneric");
    }
    
    [Fact]
    public void NullableGenericParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.SingleGenericType<int>? singleGeneric");
    }
    
    [Fact]
    public void SingleGenericNullableParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.SingleGenericType<int?> singleGeneric");
    }
    
    [Fact]
    public void DoubleGenericNullableParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.DoubleGenericType<int?, string?> doubleGeneric");
    }
    
    [Fact]
    public void NullableGenericNullableParameter()
    {
        var method = GetMethod(MethodBase.GetCurrentMethod()!.Name);
        var parameters = method.GetParameters().Select(x => x.NormalizedFullName()).ToList();
        parameters.Should()
            .NotBeEmpty().And
            .HaveCount(1).And
            .Contain("StardewAssemblyNetwork.TestAssembly.SingleGenericType<int?>? singleGeneric");
    }
}