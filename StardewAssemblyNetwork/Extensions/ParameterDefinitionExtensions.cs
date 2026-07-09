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

            List<byte>? directNullability = null;
            List<byte>? contextualNullability = null;
            bool isNullableByType = param.IsNullableByType();
            bool isNullableByAttribute = param.IsNullableByAttribute(out directNullability);
            bool isNullableByContext = !isNullableByAttribute && param.IsNullableByContext(method, out contextualNullability);
            bool isNullable = isNullableByType || isNullableByAttribute || isNullableByContext;

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
                    goto afterGenerics;
                }

                List<string> args = [];
                // foreach (var arg in genericArgs)
                // {
                //     StringBuilder argName = new StringBuilder(arg.Resolve().NormalizedFullName());
                //     if (resolvedGenericType.IsNullable(out _)) sb.Append('?');
                //     args.Add(argName.ToString());
                // }
                
                foreach (var arg in generic.GenericArguments)
                {
                    var resolved = arg.Resolve();
                    bool isParamNullable = param.IsNullable(out var paramNullability, method);
                    bool isArgNullable = resolved.IsNullable(out var argNullability, param);
                    if (arg is GenericInstanceType gType)
                    {
                        args.Add(arg.NormalizedFullName());
                    }
                    else
                    {
                        StringBuilder name = new StringBuilder(arg.NormalizedFullName());
                        if (isArgNullable || isNullableByAttribute) name.Append('?');
                        args.Add(name.ToString());
                    }
                }

                if (param.IsNullableByType())
                {
                    typeName = string.Join(", ", args);
                }
                else if (generic.DeclaringType is not null)
                {
                    typeName = $"{generic.DeclaringType.Resolve().NormalizedFullName()}<{string.Join(", ", args)}>";
                }
                else
                {
                    typeName = $"{generic.Namespace}.{generic.Resolve().NameWithoutGenerics()}<{string.Join(", ", args)}>";
                }
                Console.WriteLine("Got here");
            }
            else typeName = param.ParameterType.Resolve().NormalizedFullName();
            
            afterGenerics:
            if (param.IsDynamic() && typeName == "object") typeName = "dynamic";

            sb.Append(typeName);
            if (isNullable)
            {
                var nullability = directNullability ?? contextualNullability ?? [];
                if (param.ParameterType is ArrayType array)
                {
                    var resolvedArray = array.Resolve();
                    bool isArrayNullable = resolvedArray.IsNullable(out var arrayNullability, param);
                    if (isNullableByContext && contextualNullability?.Count == 1)
                    {
                        if (isArrayNullable) sb.Append('?');
                        sb.Append("[]?");
                    }
                    else
                    {
                        if ((arrayNullability ?? nullability).ElementAtOrDefault(1) is 2) sb.Append('?');
                        sb.Append("[]");
                        if ((arrayNullability ?? nullability).ElementAtOrDefault(0) is 2) sb.Append('?');
                    }
                }
                else if (!isNullableByAttribute) sb.Append('?');
            }
            else
            {
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
    }
}