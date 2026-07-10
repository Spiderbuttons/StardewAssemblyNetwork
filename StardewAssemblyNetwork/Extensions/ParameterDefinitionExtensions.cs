using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class ParameterDefinitionExtensions
{
    extension(ParameterDefinition param)
    {
        public string NormalizedFullName(ICustomAttributeProvider method)
        {
            StringBuilder sb = new StringBuilder();
            
            if (param.IsIn && !param.IsReadOnly()) sb.Append("in ");
            if (param.IsOut) sb.Append("out ");
            if (param.IsByReference()) sb.Append("ref ");
            if (param.IsReadOnly()) sb.Append("readonly ");

            string typeName;
            if (param.ParameterType is GenericInstanceType or ArrayType { ElementType: GenericInstanceType })
            {
                GenericInstanceType generic = param switch 
                {
                    { ParameterType: GenericInstanceType g } => g,
                    { ParameterType: ArrayType { ElementType: GenericInstanceType g } } => g,
                    _ => throw new InvalidOperationException("Parameter type is not a generic instance or array of generic instance.")
                };
                
                if (!generic.HasGenericArguments)
                {
                    typeName = generic.Resolve().NormalizedFullName();
                    goto afterGenerics;
                }

                List<string> args = [];
                args.AddRange(generic.GenericArguments.Select(arg => arg.NormalizedFullName()));
                
                if (generic.DeclaringType is not null)
                {
                    typeName = $"{generic.DeclaringType.Resolve().NormalizedFullName()}<{string.Join(", ", args)}>";
                }
                else
                {
                    typeName = $"{generic.Namespace}.{generic.Resolve().NameWithoutGenerics()}<{string.Join(", ", args)}>";
                }
            }
            else typeName = param.ParameterType.Resolve().NormalizedFullName();
            
            afterGenerics:
            if (param.IsDynamic() && typeName == "object") typeName = "dynamic";

            sb.Append(typeName);
            if (param.ParameterType is ArrayType) sb.Append("[]");

            sb.Append(' ');
            sb.Append(param.Name);

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