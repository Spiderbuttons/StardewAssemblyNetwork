using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class ParameterDefinitionExtensions
{
    extension(ParameterDefinition param)
    {
        public string NormalizedName()
        {
            return param.Name;
        }
        
        public string NormalizedFullName()
        {
            return $"{param.ParameterType.Resolve().NormalizedFullName()} {param.Name}";
        }
    }
}