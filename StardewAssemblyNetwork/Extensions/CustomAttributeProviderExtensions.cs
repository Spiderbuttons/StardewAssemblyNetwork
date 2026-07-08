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
    }
}