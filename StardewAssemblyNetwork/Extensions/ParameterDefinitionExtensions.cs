using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class ParameterDefinitionExtensions
{
    extension(ParameterDefinition param)
    {
        public string NormalizedFullName(MethodDefinition? method = null)
        {
            StringBuilder sb = new StringBuilder();
            
            if (param.IsIn && !param.IsReadOnly()) sb.Append("in ");
            if (param.IsOut) sb.Append("out ");
            if (param.IsByReference()) sb.Append("ref ");
            if (param.IsReadOnly()) sb.Append("readonly ");

            string typeName;
            if (param.ParameterType is GenericInstanceType { HasGenericArguments: true } generic)
            {
                var args = generic.GenericArguments.Select(arg => arg.Resolve().NormalizedFullName());
                if (param.IsNullableByType())
                {
                    typeName = $"{string.Join(", ", args)}";
                }
                else if (generic.DeclaringType is not null)
                {
                    typeName = $"{generic.DeclaringType.Resolve().NormalizedFullName()}<{string.Join(", ", args)}>";
                }
                else
                {
                    typeName = $"{generic.Namespace}.{generic.Resolve().NameWithoutGenerics()}<{string.Join(", ", args)}>";
                }
            }
            else typeName = param.ParameterType.Resolve().NormalizedFullName();
            if (param.IsDynamic() && typeName == "object") typeName = "dynamic";
            sb.Append(typeName);

            if (param.ParameterType is ArrayType) sb.Append("[]");

            if (param.IsNullable(method)) sb.Append('?');

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

        public bool IsNullable(MethodDefinition? method = null)
        {
            if (param.ParameterType is not ArrayType array)
            {
                return (param as ICustomAttributeProvider).IsNullable(method);
            }

            return array.ElementType.Resolve().IsNullable(method);
        }
    }
}