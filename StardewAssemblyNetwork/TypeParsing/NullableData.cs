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

    public bool ShouldUseContext()
    {
        return ArrayData is null && SingleByteData is null && NullableContext is not null;
    }

    public byte GetContextualByte()
    {
        return NullableContext?.SingleByteData ?? throw new InvalidOperationException("No contextual nullability data available.");
    }

    public static NullableData GetNullabilityData(ICustomAttributeProvider provider, ICustomAttributeProvider? parentContext = null)
    {
        object? byteData = null;
        if (provider.TryGetAttribute("NullableAttribute", out var directAttr) && directAttr.HasConstructorArguments)
        {
            if (directAttr.ConstructorArguments[0].Value is CustomAttributeArgument[] args)
            {
                if (args.Length == 1) byteData = (byte)args[0].Value;
                else byteData = args.Select(arg => arg.Value).OfType<byte>().ToArray();
            }
            else byteData = directAttr.ConstructorArguments[0].Value;
        }
        
        NullableData result = new NullableData(provider, byteData, false);
        ICustomAttributeProvider? parentContextProvider = parentContext ?? provider switch 
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
            var args = (directAttr.ConstructorArguments[0].Value as CustomAttributeArgument[])!;
            if (args.Length == 1) byteData = (byte)args[0].Value;
            else byteData = args.Select(arg => arg.Value).OfType<byte>().ToArray();
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

    // private static NullableData GetNullabilityForArrayType(TypeDefinition array, ICustomAttributeProvider parentContext)
    // {
    //     if (!array.IsArray) throw new ArgumentException("TypeDefinition is not an array type.", nameof(array));
    //     object? byteData = null;
    //     if (array.TryGetAttribute("NullableAttribute", out var directAttr) && directAttr.HasConstructorArguments)
    //     {
    //         if (directAttr.ConstructorArguments[0].Value is CustomAttributeArgument[] args)
    //         {
    //             if (args.Length == 1) byteData = (byte)args[0].Value;
    //             else byteData = args.Select(arg => arg.Value).OfType<byte>().ToArray();
    //         }
    //         else byteData = directAttr.ConstructorArguments[0].Value;
    //     }
    //     
    //     NullableData result = new NullableData(array, byteData, false);
    //     ICustomAttributeProvider? parentContextProvider = parentContext;
    // }
}