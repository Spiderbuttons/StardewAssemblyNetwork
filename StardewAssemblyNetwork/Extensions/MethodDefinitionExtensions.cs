using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class MethodDefinitionExtensions
{
    extension(MethodDefinition method)
    {
        public List<ParameterDefinition> GetParameters()
        {
            return !method.HasParameters ? [] : method.Parameters.ToList();
        }
    }
}