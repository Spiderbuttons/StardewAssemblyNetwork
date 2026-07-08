using FluentAssertions;
using Mono.Cecil;
using StardewAssemblyNetwork.Extensions;

namespace StardewAssemblyNetwork.Tests;

[Collection("TestContext")]
public class FieldPropertyTests
{
    private const string TEST_TYPE_NAMESPACE = "StardewAssemblyNetwork.TestAssembly";
    private const string TEST_CLASS = "FieldPropertyTests";
    private const string FULL_CLASS_NAME = $"{TEST_TYPE_NAMESPACE}.{TEST_CLASS}";

    private readonly TypeDefinition TestType;
    
    private readonly TestContext context;

    public FieldPropertyTests(TestContext ctx)
    {
        context = ctx;
        TestType = context.TestAssembly.GetType(FULL_CLASS_NAME);
    }

    private PropertyDefinition GetProperty(string name)
    {
        return TestType.GetProperty(name) ?? throw new ArgumentException($"Property '{name}' not found in type '{TestType.FullName}'");
    }

    private FieldDefinition GetField(string name)
    {
        return TestType.GetField(name) ?? throw new ArgumentException($"Field '{name}' not found in type '{TestType.FullName}'");
    }
    
    [Fact]
    public void GetterSetterProperty()
    {
        var property = GetProperty("GetterSetterProperty");
        property.NormalizedName().Should().Be("GetterSetterProperty { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.GetterSetterProperty {{ get; set; }}");
    }
    
    [Fact]
    public void GetterSetterDefinedProperty()
    {
        var property = GetProperty("GetterSetterDefinedProperty");
        property.NormalizedName().Should().Be("GetterSetterDefinedProperty { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.GetterSetterDefinedProperty {{ get; set; }}");
    }
    
    [Fact]
    public void GetterOnlyProperty()
    {
        var property = GetProperty("GetterOnlyProperty");
        property.NormalizedName().Should().Be("GetterOnlyProperty { get; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.GetterOnlyProperty {{ get; }}");
    }
    
    [Fact]
    public void SetterOnlyProperty()
    {
        var property = GetProperty("SetterOnlyProperty");
        property.NormalizedName().Should().Be("SetterOnlyProperty { set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.SetterOnlyProperty {{ set; }}");
    }
    
    [Fact]
    public void AutoProperty()
    {
        var property = GetProperty("AutoProperty");
        property.NormalizedName().Should().Be("AutoProperty { get; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.AutoProperty {{ get; }}");
    }
    
    [Fact]
    public void get_Property()
    {
        var property = GetProperty("get_Property");
        property.NormalizedName().Should().Be("get_Property { get; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.get_Property {{ get; }}");
    }
    
    [Fact]
    public void set_Property()
    {
        var property = GetProperty("set_Property");
        property.NormalizedName().Should().Be("set_Property { get; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.set_Property {{ get; }}");
    }
    
    [Fact]
    public void get_Item()
    {
        var property = GetProperty("get_Item");
        property.NormalizedName().Should().Be("get_Item { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.get_Item {{ get; set; }}");
    }
    
    [Fact]
    public void set_Item()
    {
        var property = GetProperty("set_Item");
        property.NormalizedName().Should().Be("set_Item { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.set_Item {{ get; set; }}");
    }
    
    [Fact]
    public void NestedPropertyValue()
    {
        var type = TestType.GetType("NestedProperty", false) ?? throw new ArgumentException($"Nested type 'NestedProperty' not found in type '{TestType.FullName}'");
        var property = type.GetProperty("NestedPropertyValue") ?? throw new ArgumentException($"Property 'NestedPropertyValue' not found in type '{type.FullName}'");
        property.NormalizedName().Should().Be("NestedPropertyValue { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}+NestedProperty.NestedPropertyValue {{ get; set; }}");
    }
    
    [Fact]
    public void SingleGenericProperty()
    {
        var property = GetProperty("SingleGenericProperty");
        property.NormalizedName().Should().Be("SingleGenericProperty { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.SingleGenericProperty {{ get; set; }}");
    }
    
    [Fact]
    public void DoubleGenericProperty()
    {
        var property = GetProperty("DoubleGenericProperty");
        property.NormalizedName().Should().Be("DoubleGenericProperty { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.DoubleGenericProperty {{ get; set; }}");
    }
    
    [Fact]
    public void NullableValueTypeProperty()
    {
        var property = GetProperty("NullableValueTypeProperty");
        property.NormalizedName().Should().Be("NullableValueTypeProperty { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.NullableValueTypeProperty {{ get; set; }}");
    }
    
    [Fact]
    public void NullableReferenceTypeProperty()
    {
        var property = GetProperty("NullableReferenceTypeProperty");
        property.NormalizedName().Should().Be("NullableReferenceTypeProperty { get; set; }");
        property.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.NullableReferenceTypeProperty {{ get; set; }}");
    }
    
    [Fact]
    public void valueField()
    {
        var field = GetField("valueField");
        field.NormalizedName().Should().Be("valueField");
        field.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.valueField");
    }
    
    [Fact]
    public void referenceField()
    {
        var field = GetField("referenceField");
        field.NormalizedName().Should().Be("referenceField");
        field.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.referenceField");
    }
    
    [Fact]
    public void singleGenericField()
    {
        var field = GetField("singleGenericField");
        field.NormalizedName().Should().Be("singleGenericField");
        field.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.singleGenericField");
    }
    
    [Fact]
    public void doubleGenericField()
    {
        var field = GetField("doubleGenericField");
        field.NormalizedName().Should().Be("doubleGenericField");
        field.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.doubleGenericField");
    }
    
    [Fact]
    public void nullableValueTypeField()
    {
        var field = GetField("nullableValueTypeField");
        field.NormalizedName().Should().Be("nullableValueTypeField");
        field.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.nullableValueTypeField");
    }
    
    [Fact]
    public void nullableReferenceTypeField()
    {
        var field = GetField("nullableReferenceTypeField");
        field.NormalizedName().Should().Be("nullableReferenceTypeField");
        field.NormalizedFullName().Should().Be($"{FULL_CLASS_NAME}.nullableReferenceTypeField");
    }
    
    [Fact]
    public void Indexer()
    {
        var type = context.TestAssembly.GetType($"{TEST_TYPE_NAMESPACE}.IndexerTests");
        var property = type.GetProperty("Item") ?? throw new ArgumentException($"Property 'get_Item' not found in type '{type.FullName}'");
        property.IsIndexer().Should().BeTrue();
        property.NormalizedName().Should().Be("Indexer[int index]");
        property.NormalizedFullName().Should().Be($"{TEST_TYPE_NAMESPACE}.IndexerTests::Indexer[int index]");
    }
}