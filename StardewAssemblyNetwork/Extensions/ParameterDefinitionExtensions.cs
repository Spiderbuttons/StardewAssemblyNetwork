using System.Text;
using Mono.Cecil;
using StardewAssemblyNetwork.TypeParsing;

namespace StardewAssemblyNetwork.Extensions;

public static class ParameterDefinitionExtensions
{
    extension(ParameterDefinition param)
    {
        public string NormalizedFullName(MethodDefinition method)
        {
            StringBuilder sb = new StringBuilder();
            
            if (param.IsIn && !param.IsReadOnly()) sb.Append("in ");
            if (param.IsOut) sb.Append("out ");
            if (param.IsByReference()) sb.Append("ref ");
            if (param.IsReadOnly()) sb.Append("readonly ");

            string typeName;
            if (param.ParameterType.FullName.StartsWith("System.Nullable"))
            {
                typeName = param.ParameterType.NormalizedFullName();
            }
            else if (param.ParameterType is GenericInstanceType or ArrayType { ElementType: GenericInstanceType })
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
                    typeName = $"{generic.DeclaringType.NormalizedFullName()}<{string.Join(", ", args)}>";
                }
                else
                {
                    typeName = $"{generic.Namespace}.{generic.NameWithoutGenerics()}<{string.Join(", ", args)}>";
                }
            }
            else typeName = param.ParameterType.Resolve().NormalizedFullName();
            
            afterGenerics:
            if (param.IsDynamic() && typeName == "object") typeName = "dynamic";

            sb.Append(typeName);
            if (param.ParameterType is ArrayType) sb.Append("[]");

            NullableData nullableData = NullableData.GetNullabilityData(param, method);
            if (nullableData.SingleByteData is null && nullableData.ArrayData is null &&
                nullableData.NullableContext is { } ctx)
            {
                if (ctx.SingleByteData is 2) sb.Append('?');
            }

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