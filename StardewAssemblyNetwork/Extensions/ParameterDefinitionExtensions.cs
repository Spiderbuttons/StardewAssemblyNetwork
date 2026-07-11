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
            NullableData nullableData = NullableData.GetNullabilityData(param, method);
            NullableData? arrayNullableData = null;
            if (param.ParameterType is ArrayType array)
            {
                var resolvedArray = array.Resolve();
                arrayNullableData = NullableData.GetNullabilityData(resolvedArray);
            }
            
            bool shouldUseContextualNull = nullableData.SingleByteData is null && nullableData.ArrayData is null && nullableData.NullableContext is not null;
            
            if (param.IsIn && !param.IsReadOnly()) sb.Append("in ");
            if (param.IsOut) sb.Append("out ");
            if (param.IsByReference()) sb.Append("ref ");
            if (param.IsReadOnly()) sb.Append("readonly ");

            string typeName;
            if (param.ParameterType.FullName.StartsWith("System.Nullable"))
            {
                if (param.ParameterType is ArrayType nullableArray)
                {
                    if (!nullableArray.ElementType.IsValueType)
                    {
                        typeName = $"{nullableArray.ElementType.NormalizedFullName()}";
                    }
                    else
                    {
                        typeName = $"{nullableArray.ElementType.NormalizedFullName()}[]";
                    }
                }
                else typeName = param.ParameterType.NormalizedFullName();
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
                args.AddRange(generic.GenericArguments.Select(arg =>
                {
                    if (arg.FullName.StartsWith("System.Nullable")) return arg.NormalizedFullName();

                    var argIndex = generic.GenericArguments.IndexOf(arg) + 1;
                    if (argIndex >= nullableData.ArrayData?.Length)
                    {
                        // Idk why this happens sometimes, I guess sometimes the first byte gets wrapped into context... ?
                        argIndex--;
                    }
                    
                    if (nullableData.SingleByteData is 2 || 
                        nullableData.ArrayData?.ElementAtOrDefault(argIndex) is 2)
                    {
                        return $"{arg.NormalizedFullName()}?";
                    }
                    
                    return arg.NormalizedFullName();
                }));

                if (!generic.FullName.StartsWith("System.ValueTuple"))
                {
                    if (generic.DeclaringType is not null)
                    {
                        typeName = $"{generic.DeclaringType.NormalizedFullName()}<{string.Join(", ", args)}>";
                    }
                    else
                    {
                        typeName = $"{generic.Namespace}.{generic.NameWithoutGenerics()}<{string.Join(", ", args)}>";
                    }
                }
                else
                {
                    typeName = $"({string.Join(", ", args)})";
                }
            }
            else
            {
                typeName = param.ParameterType.Resolve().NormalizedFullName();
            }
            
            afterGenerics:
            if (param.IsDynamic() && typeName == "object") typeName = "dynamic";

            sb.Append(typeName);
            if (!param.ParameterType.FullName.StartsWith("System.Nullable") && param.ParameterType is ArrayType arr)
            {
                var resolvedArray = arr.Resolve();
                var resolvedElement = resolvedArray.GetElementType().Resolve();
                NullableData elementNullability = NullableData.GetNullabilityData(resolvedElement, resolvedArray);
                bool shouldUseElementContext = elementNullability.SingleByteData is null && elementNullability.ArrayData is null && elementNullability.NullableContext is not null;

                if (resolvedElement.IsValueType)
                {
                    if (elementNullability.ShouldUseContext() && elementNullability.GetContextualByte() is 2 || elementNullability.SingleByteData is 2 || elementNullability.ArrayData?.ElementAtOrDefault(0) is 2)
                    {
                        sb.Append('?');
                    }
                }
                else if ((nullableData.ShouldUseContext() && nullableData.GetContextualByte() is 2) ||
                    nullableData.SingleByteData is 2 || nullableData.ArrayData?.ElementAtOrDefault(1) is 2)
                {
                    sb.Append('?');
                }
                
                sb.Append("[]");
                if ((nullableData.ShouldUseContext() && nullableData.GetContextualByte() is 2) ||
                    nullableData.SingleByteData is 2 || nullableData.ArrayData?.ElementAtOrDefault(0) is 2)
                {
                    sb.Append('?');
                }
            } else {
                if (shouldUseContextualNull) {
                    if (nullableData.NullableContext?.SingleByteData is 2) sb.Append('?');
                }
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