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
            // if (param.IsNullableByType())
            // {
            //     if (param.ParameterType is GenericInstanceType { HasGenericArguments: true } generic)
            //     {
            //         var args = generic.GenericArguments.Select(arg => arg.Resolve().NormalizedFullName());
            //         typeName = $"{string.Join(", ", args)}";
            //     } else if (param.ParameterType is ArrayType array)
            //     {
            //         var args = array.ElementType.GenericParameters.Select(arg => arg.Resolve().NormalizedFullName());
            //         typeName = $"{string.Join(", ", args)}";
            //     }
            //     else typeName = param.ParameterType.Resolve().NormalizedFullName();
            // }
            // else 
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
                }
                else
                {

                    var args = generic.GenericArguments.Select(arg => arg.Resolve().NormalizedFullName());
                    // if (param.IsNullableByType())
                    // {
                    //     typeName = $"{string.Join(", ", args)}";
                    // }
                    if (generic.DeclaringType is not null)
                    {
                        typeName = $"{generic.DeclaringType.Resolve().NormalizedFullName()}<{string.Join(", ", args)}>";
                    }
                    else
                    {
                        typeName = $"{generic.Namespace}.{generic.Resolve().NameWithoutGenerics()}<{string.Join(", ", args)}>";
                    }
                }
            }
            else typeName = param.ParameterType.Resolve().NormalizedFullName();
            if (param.IsDynamic() && typeName == "object") typeName = "dynamic";

            if (param.IsNullable(out var nullability, method))
            {
                sb.Append(typeName);
                if (param.ParameterType is ArrayType array)
                {
                    var res = param.ParameterType;
                    if (nullability?.Count == 1)
                    {
                        if (!array.ElementType.IsValueType) sb.Append('?');
                        sb.Append("[]?");
                    }
                    else
                    {
                        if (nullability?.ElementAtOrDefault(1) is 2) sb.Append('?');
                        sb.Append("[]");
                        if (nullability?.ElementAtOrDefault(0) is 2) sb.Append('?');
                    }
                }
                else sb.Append('?');
            }
            else
            {
                sb.Append(typeName);
                if (param.ParameterType is ArrayType) sb.Append("[]");
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

        // public bool IsNullable(ICustomAttributeProvider method)
        // {
        //     return (param as ICustomAttributeProvider).IsNullable(method);
        //     
        //     if (param.ParameterType is not ArrayType array)
        //     {
        //         return (param as ICustomAttributeProvider).IsNullable(method);
        //     }
        //
        //     var resolved = array.Resolve();
        //     return resolved.IsNullable(method);
        // }
    }
}