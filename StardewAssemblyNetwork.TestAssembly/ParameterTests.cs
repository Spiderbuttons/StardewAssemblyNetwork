using System.Runtime.InteropServices;

namespace StardewAssemblyNetwork.TestAssembly;

public class ParameterTests
{
    public struct StructType;
    
    public void NoParameters() {}

    public void OneParameter(int index) {}
    
    public void TwoParameters(int index1, int index2) {}
    
    public void ReferenceParameter(ParameterTests parameter) {}
    
    public void StructParameter(StructType structType) {}
    
    public void InParameter(in int index) {}

    public void OutParameter(out int index)
    {
        index = 1;
    }
    
    public void RefParameter(ref int index) {}
    
    public void InOutParameter([In, Out] int index) {}
    
    public void RefReadonlyParameter(ref readonly int index) {}
    
    public void OptionalParameter(int index = 1) {}
    
    public void ObjectParameter(object index) {}

    public void DynamicParameter(dynamic index) {}
    
    public void NullableValueTypeParameter(int? index) {}
    
    public void NullableReferenceTypeParameter(ParameterTests? parameter) {}
    
    public void NullableStructTypeParameter(StructType? parameter) {}
    
    public void AllNullableParameters(int? index1, int? index2, int? index3) {}
    
    public void MostlyNullableParameters(int? index1, int? index2, int index3) {}
    
    public void MostlyNonNullableParameters(int? index1, int index2, int index3) {}
    
    public void ValueTypeArrayParameter(int[] indexes) {}
    
    public void ReferenceTypeArrayParameter(string[] strings) {}
    
    public void NullableArrayParameter(string[]? strings) {}
    
    public void NullableNullableArrayParameter(string?[]? strings) {}
    
    public void SingleGenericParameter(SingleGenericType<int> singleGeneric) {}
    
    public void DoubleGenericParameter(DoubleGenericType<int, string> doubleGeneric) {}
    
    public void NullableGenericParameter(SingleGenericType<int>? singleGeneric) {}
    
    public void SingleGenericNullableParameter(SingleGenericType<int?> singleGeneric) {}
    
    public void DoubleGenericNullableParameter(DoubleGenericType<int?, string?> doubleGeneric) {}
    
    public void NullableGenericNullableParameter(SingleGenericType<int?>? singleGeneric) {}
}