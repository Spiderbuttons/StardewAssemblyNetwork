using System.Diagnostics.CodeAnalysis;
using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class GenericExtensions
{
    extension(GenericInstanceType type)
    {
        public List<TypeReference> GetGenericArguments()
        {
            return type.GenericArguments.ToList();
        }
    }
}