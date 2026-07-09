using System.Diagnostics.CodeAnalysis;
using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class CustomAttributeProviderExtensions
{
    extension(ICustomAttributeProvider provider)
    {
        public bool HasAttribute(string name)
        {
            return provider.HasCustomAttributes && provider.CustomAttributes.Any(attr => attr.AttributeType.FullName == name || attr.AttributeType.Name == name);
        }

        public bool TryGetAttribute(string name, [NotNullWhen(true)] out CustomAttribute? attribute)
        {
            if (provider.HasCustomAttributes)
            {
                attribute = provider.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.FullName == name || attr.AttributeType.Name == name);
                return attribute is not null;
            }

            attribute = null;
            return false;
        }

        public bool IsDynamic()
        {
            return provider.TryGetAttribute("DynamicAttribute", out _);
        }

        public bool IsNullable(out List<byte>? nullability, ICustomAttributeProvider? parentContext = null)
        {
            nullability = null;
            bool isNullableByType = provider.IsNullableByType();
            bool isNullableByAttribute = provider.IsNullableByAttribute(out var attributeNullability);
            bool isNullableByContext = provider.IsNullableByContext(parentContext, out var contextNullability);
            nullability = attributeNullability ?? contextNullability;
            return isNullableByType || isNullableByAttribute || isNullableByContext;
        }

        public bool IsNullableByType()
        {
            return provider switch
            {
                MemberReference refer => refer.FullName.StartsWith("System.Nullable"),
                ParameterReference param => param.ParameterType.FullName.StartsWith("System.Nullable"),
                _ => false
            };
        }

        public bool IsNullableByAttribute([NotNullWhen(true)] out List<byte>? nullability)
        {
            nullability = null;
            if (!provider.TryGetAttribute("NullableAttribute", out CustomAttribute? attr) || !attr.HasConstructorArguments)
            {
                return false;
            }

            TypeReference? type = provider switch
            {
                ParameterDefinition param => param.ParameterType,
                ParameterReference param => param.ParameterType.Resolve(),
                _ => null
            };
            
            var val = attr.ConstructorArguments[0].Value;

            if (type is null) goto fallback;
            if (type.IsArray && val is CustomAttributeArgument[] bytes)
            {
                bool foundTrue = false;
                foreach (var byteArg in bytes)
                {
                    if (byteArg.Value is byte b)
                    {
                        nullability ??= [];
                        nullability.Add(b);
                        if (b == 2) foundTrue = true;
                    }
                }
                return foundTrue;
            }

            if (type.IsGenericInstance && val is CustomAttributeArgument[] bytes2)
            {
                bool foundTrue = false;
                foreach (var byteArg in bytes2)
                {
                    if (byteArg.Value is byte b)
                    {
                        nullability ??= [];
                        nullability.Add(b);
                        if (b == 2) foundTrue = true;
                    }
                }
                return foundTrue;
            }

            // if (type.IsGenericInstance && val is CustomAttributeArgument[] bytes2)
            // {
            //     bool foundTrue = false;
            //     for (int i = 0; i < bytes2.Length; i++)
            //     {
            //         if (bytes2[i].Value is byte and 2) foundTrue = true;
            //     }
            // }

            fallback:
            return attr.ConstructorArguments[0].Value switch
            {
                byte[] byteArray => byteArray.Length > 0 && byteArray[0] == 2,
                byte singleByte => singleByte == 2,
                _ => false
            };
        }
        
        public bool IsNullableByContext([NotNullWhen(true)] out List<byte>? nullability)
        {
            nullability = null;
            if (!provider.TryGetAttribute("NullableContextAttribute", out CustomAttribute? attr) ||
                !attr.HasConstructorArguments) return false;

            if (attr.ConstructorArguments[0].Value is not (byte)2) return false;

            nullability = [2];
            return true;
        }
        
        public bool IsNullableByContext(ICustomAttributeProvider? parentContext, [NotNullWhen(true)] out List<byte>? nullability)
        {
            nullability = null;
            if (provider.IsNullableByContext(out var contextNullability)) 
            {
                nullability = contextNullability;
                return true;
            }

            ICustomAttributeProvider? newParent = provider switch 
            {
                MethodDefinition method => method.DeclaringType,
                PropertyDefinition prop => prop.DeclaringType,
                FieldDefinition field => field.DeclaringType,
                TypeDefinition { DeclaringType: not null } type => type.DeclaringType,
                TypeDefinition { DeclaringType: null } type => type.Module,
                ModuleDefinition module => module.Assembly,
                _ => null
            };
            
            return parentContext is not null && parentContext.IsNullableByContext(newParent, out nullability);
        }
    }
}