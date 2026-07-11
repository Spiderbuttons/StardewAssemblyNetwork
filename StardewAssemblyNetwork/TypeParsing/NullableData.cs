using Mono.Cecil;
using StardewAssemblyNetwork.Extensions;

namespace StardewAssemblyNetwork.TypeParsing;

public class NullableData
{
    public ICustomAttributeProvider Provider;
    public NullableData? NullableContext;
    
    public byte[]? ArrayData;
    public byte? SingleByteData;

    public bool IsContext;

    public NullableData(ICustomAttributeProvider provider, object? byteData, bool isContext)
    {
        Provider = provider;
        if (byteData is byte singleByte)
        {
            SingleByteData = singleByte;
            ArrayData = null;
        }
        else if (byteData is byte[] arrayBytes)
        {
            ArrayData = arrayBytes;
            SingleByteData = null;
        } else
        {
            ArrayData = null;
            SingleByteData = null;
        }
        IsContext = isContext;
    }

    public static NullableData GetNullabilityData(ICustomAttributeProvider provider)
    {
        object? byteData = null;
        if (provider.TryGetAttribute("NullableAttribute", out var directAttr) && directAttr.HasConstructorArguments)
        {
            byteData = directAttr.ConstructorArguments[0].Value;
        }
        
        NullableData result = new NullableData(provider, byteData, false);
        ICustomAttributeProvider? parentContextProvider = provider switch 
        {
            MethodDefinition method => method.DeclaringType,
            IMemberDefinition { DeclaringType: not null } member => member.DeclaringType,
            TypeDefinition type => type.Module,
            ModuleDefinition module => module.Assembly,
            _ => throw new InvalidOperationException("Unsupported type for nullability context.")
        };
        
        while (result.NullableContext is null && parentContextProvider is not null)
        {
            if (parentContextProvider.TryGetAttribute("NullableContextAttribute", out var contextAttr) && contextAttr.HasConstructorArguments)
            {
                result.NullableContext = new NullableData(parentContextProvider, contextAttr.ConstructorArguments[0].Value, true);
            }
            else
            {
                parentContextProvider = parentContextProvider switch 
                {
                    MethodDefinition method => method.DeclaringType,
                    IMemberDefinition { DeclaringType: not null } member => member.DeclaringType,
                    TypeDefinition type => type.Module,
                    ModuleDefinition module => module.Assembly,
                    _ => null
                };
            }
        }
        
        return result;
    }
    
    public static NullableData GetNullabilityData(ParameterDefinition param, MethodDefinition method)
    {
        object? byteData = null;
        if (param.TryGetAttribute("NullableAttribute", out var directAttr) && directAttr.HasConstructorArguments)
        {
            byteData = directAttr.ConstructorArguments[0].Value;
        }
        
        NullableData result = new NullableData(param, byteData, false);
        ICustomAttributeProvider? parentContextProvider = method;
        
        while (result.NullableContext is null && parentContextProvider is not null)
        {
            if (parentContextProvider.TryGetAttribute("NullableContextAttribute", out var contextAttr) && contextAttr.HasConstructorArguments)
            {
                result.NullableContext = new NullableData(parentContextProvider, contextAttr.ConstructorArguments[0].Value, true);
            }
            else
            {
                parentContextProvider = parentContextProvider switch 
                {
                    MethodDefinition meth => meth.DeclaringType,
                    IMemberDefinition { DeclaringType: not null } member => member.DeclaringType,
                    TypeDefinition type => type.Module,
                    ModuleDefinition module => module.Assembly,
                    _ => null
                };
            }
        }
        
        return result;
    }
}