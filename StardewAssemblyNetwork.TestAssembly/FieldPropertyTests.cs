namespace StardewAssemblyNetwork.TestAssembly;

public class FieldPropertyTests
{
    public int GetterSetterProperty { get; set; }
    
    public int GetterSetterDefinedProperty
    {
        get { return GetterSetterProperty; }
        set { GetterSetterProperty = value; }
    }
    
    public int GetterOnlyProperty { get; }
    
    public int SetterOnlyProperty { set { GetterSetterProperty = value; } }
    
    public int AutoProperty => GetterSetterProperty;
    
    public int get_Property => GetterSetterProperty;
    
    public int set_Property => GetterSetterProperty;
    
    public int get_Item { get; set; }
    
    public int set_Item { get; set; }

    public class NestedProperty
    {
        public int NestedPropertyValue { get; set; }
    }
    
    public SingleGenericType<int> SingleGenericProperty { get; set; }
    
    public DoubleGenericType<int, string> DoubleGenericProperty { get; set; }
    
    public int? NullableValueTypeProperty { get; set; }
    
    public NormalType? NullableReferenceTypeProperty { get; set; }

    public int valueField;

    public NormalType referenceField;
    
    public SingleGenericType<int> singleGenericField;
    
    public DoubleGenericType<int, string> doubleGenericField;
    
    public int? nullableValueTypeField;
    
    public NormalType? nullableReferenceTypeField;
}

public class IndexerTests
{
    public int this[int index]
    {
        get { return index; }
        set {  }
    }
}