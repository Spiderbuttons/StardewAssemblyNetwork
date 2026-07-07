using Mono.Cecil;
using StardewAssemblyNetwork.Extensions;

namespace StardewAssemblyNetwork.Tests;

[Collection("TestContext")]
public class FieldPropertyTests
{
    private const string TEST_TYPE_NAMESPACE = "StardewAssemblyNetwork.TestAssembly";
    private const string TEST_CLASS = "FieldPropertyTests";
    private string FULL_CLASS_NAME => $"{TEST_TYPE_NAMESPACE}.{TEST_CLASS}";

    private TypeDefinition TestType;
    
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
        Assert.Equal("GetterSetterProperty { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.GetterSetterProperty {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void GetterSetterDefinedProperty()
    {
        var property = GetProperty("GetterSetterDefinedProperty");
        Assert.Equal("GetterSetterDefinedProperty { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.GetterSetterDefinedProperty {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void GetterOnlyProperty()
    {
        var property = GetProperty("GetterOnlyProperty");
        Assert.Equal("GetterOnlyProperty { get; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.GetterOnlyProperty {{ get; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void SetterOnlyProperty()
    {
        var property = GetProperty("SetterOnlyProperty");
        Assert.Equal("SetterOnlyProperty { set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.SetterOnlyProperty {{ set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void AutoProperty()
    {
        var property = GetProperty("AutoProperty");
        Assert.Equal("AutoProperty { get; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.AutoProperty {{ get; }}", property.NormalizedFullName());
    }
    
    
    [Fact]
    public void get_Property()
    {
        var property = GetProperty("get_Property");
        Assert.Equal("get_Property { get; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.get_Property {{ get; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void set_Property()
    {
        var property = GetProperty("set_Property");
        Assert.Equal("set_Property { get; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.set_Property {{ get; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void get_Item()
    {
        var property = GetProperty("get_Item");
        Assert.Equal("get_Item { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.get_Item {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void set_Item()
    {
        var property = GetProperty("set_Item");
        Assert.Equal("set_Item { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.set_Item {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void NestedPropertyValue()
    {
        var type = TestType.GetType("NestedProperty", false) ?? throw new ArgumentException($"Nested type 'NestedProperty' not found in type '{TestType.FullName}'");
        var property = type.GetProperty("NestedPropertyValue") ?? throw new ArgumentException($"Property 'NestedPropertyValue' not found in type '{type.FullName}'");
        Assert.Equal("NestedPropertyValue { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}+NestedProperty.NestedPropertyValue {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void SingleGenericProperty()
    {
        var property = GetProperty("SingleGenericProperty");
        Assert.Equal("SingleGenericProperty { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.SingleGenericProperty {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void DoubleGenericProperty()
    {
        var property = GetProperty("DoubleGenericProperty");
        Assert.Equal("DoubleGenericProperty { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.DoubleGenericProperty {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void NullableValueTypeProperty()
    {
        var property = GetProperty("NullableValueTypeProperty");
        Assert.Equal("NullableValueTypeProperty { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.NullableValueTypeProperty {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void NullableReferenceTypeProperty()
    {
        var property = GetProperty("NullableReferenceTypeProperty");
        Assert.Equal("NullableReferenceTypeProperty { get; set; }", property.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.NullableReferenceTypeProperty {{ get; set; }}", property.NormalizedFullName());
    }
    
    [Fact]
    public void valueField()
    {
        var field = GetField("valueField");
        Assert.Equal("valueField", field.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.valueField", field.NormalizedFullName());
    }
    
    [Fact]
    public void referenceField()
    {
        var field = GetField("referenceField");
        Assert.Equal("referenceField", field.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.referenceField", field.NormalizedFullName());
    }
    
    [Fact]
    public void singleGenericField()
    {
        var field = GetField("singleGenericField");
        Assert.Equal("singleGenericField", field.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.singleGenericField", field.NormalizedFullName());
    }
    
    [Fact]
    public void doubleGenericField()
    {
        var field = GetField("doubleGenericField");
        Assert.Equal("doubleGenericField", field.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.doubleGenericField", field.NormalizedFullName());
    }
    
    [Fact]
    public void nullableValueTypeField()
    {
        var field = GetField("nullableValueTypeField");
        Assert.Equal("nullableValueTypeField", field.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.nullableValueTypeField", field.NormalizedFullName());
    }
    
    [Fact]
    public void nullableReferenceTypeField()
    {
        var field = GetField("nullableReferenceTypeField");
        Assert.Equal("nullableReferenceTypeField", field.NormalizedName());
        Assert.Equal($"{FULL_CLASS_NAME}.nullableReferenceTypeField", field.NormalizedFullName());
    }
    
    [Fact]
    public void Indexer()
    {
        var type = context.TestAssembly.GetType($"{TEST_TYPE_NAMESPACE}.IndexerTests");
        var property = type.GetProperty("Item") ?? throw new ArgumentException($"Property 'get_Item' not found in type '{type.FullName}'");
        Assert.Equal("Indexer[int index]", property.NormalizedName());
        Assert.Equal($"{TEST_TYPE_NAMESPACE}.IndexerTests::Indexer[int index]", property.NormalizedFullName());
    }
}