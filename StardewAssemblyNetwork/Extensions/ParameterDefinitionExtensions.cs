using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class ParameterDefinitionExtensions
{
    extension(ParameterDefinition param)
    {
        public string NormalizedName()
        {
            string normalized = param.ParameterType.Resolve().NormalizedFullName();
            if (normalized is "object" && param.IsDynamic()) normalized = "dynamic";
            return $"{normalized} {param.Name}";
        }
        
        public string NormalizedFullName()
        {
            StringBuilder sb = new StringBuilder();
            
            if (param.IsIn && !param.IsReadOnly()) sb.Append("in ");
            if (param.IsOut) sb.Append("out ");
            if (param.IsByReference()) sb.Append("ref ");
            if (param.IsReadOnly()) sb.Append("readonly ");

            sb.Append(param.NormalizedName());

            if (param is { IsOptional: true, HasConstant: true }) sb.Append($" = {param.Constant}");
            
            return sb.ToString();
        }

        public bool IsByReference()
        {
            return (param is { IsIn: false, IsOut: false } && param.ParameterType.IsByReference) || param.IsReadOnly();
        }

        public bool IsReadOnly()
        {
            return param.HasAttribute("RequiresLocationAttribute");
        }
    }
}