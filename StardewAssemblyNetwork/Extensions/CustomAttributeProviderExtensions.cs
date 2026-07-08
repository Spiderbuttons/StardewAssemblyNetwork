using System.Diagnostics.CodeAnalysis;
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

        public bool IsNullable(ICustomAttributeProvider? parentContext = null)
        {
            // check if the type of provider has an extension method called
            return provider.IsNullableByType() || provider.IsNullableByAttribute() || provider.IsNullableByContext(parentContext);
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

        public bool IsNullableByAttribute()
        {
            if (!provider.TryGetAttribute("NullableAttribute", out CustomAttribute? attr) || !attr.HasConstructorArguments)
            {
                return false;
            }

            return attr.ConstructorArguments[0].Value switch
            {
                byte[] byteArray => byteArray.Length > 0 && byteArray[0] == 2,
                byte singleByte => singleByte == 2,
                _ => false
            };
        }
        
        public bool IsNullableByContext()
        {
            if (!provider.TryGetAttribute("NullableContextAttribute", out CustomAttribute? attr) ||
                !attr.HasConstructorArguments) return false;
            
            return attr.ConstructorArguments[0].Value switch
            {
                byte singleByte => singleByte == 2,
                byte[] byteArray => byteArray.Length > 0 && byteArray[0] == 2,
                _ => false
            };
        }
        
        public bool IsNullableByContext(ICustomAttributeProvider? parentContext)
        {
            if (provider.IsNullableByContext()) return true;
            
            ICustomAttributeProvider? newParent = provider switch 
            {
                MethodDefinition method => method.DeclaringType,
                PropertyDefinition prop => prop.DeclaringType,
                FieldDefinition field => field.DeclaringType,
                TypeDefinition type => type.DeclaringType,
                _ => null
            };
            return parentContext is not null && parentContext.IsNullableByContext(newParent);
        }
    }
}