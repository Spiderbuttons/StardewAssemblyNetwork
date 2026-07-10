using System.Diagnostics.CodeAnalysis;
using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class TypeDefinitionExtensions
{
    extension(TypeReference type)
    {
        public bool TryGetBuiltInName([NotNullWhen(true)] out string? builtInName)
        { 
            builtInName = type.FullName switch
            {
                "System.Boolean" => "bool",
                "System.Byte" => "byte",
                "System.SByte" => "sbyte",
                "System.Char" => "char",
                "System.Decimal" => "decimal",
                "System.Double" => "double",
                "System.Single" => "float",
                "System.Int32" => "int",
                "System.UInt32" => "uint",
                "System.IntPtr" => "nint",
                "System.UIntPtr" => "nuint",
                "System.Int64" => "long",
                "System.UInt64" => "ulong",
                "System.Int16" => "short",
                "System.UInt16" => "ushort",
                "System.String" => "string",
                "System.Object" when type.Resolve().IsDynamic() => "dynamic",
                "System.Object" when !type.Resolve().IsDynamic() => "object",
                _ => null
            };
            if (builtInName is not null) return true;
            if (!type.FullName.StartsWith("System.Nullable") || type is not GenericInstanceType nullableType) return false;

            var testthing = "test";

            return false;
        }

        public string NameWithoutGenerics()
        {
            return type.Name[..type.Name.IndexOf('`')];
        }
        
        public string NormalizedName()
        {
            if (type.TryGetBuiltInName(out string? builtInName)) return builtInName;

            if (type is not IGenericInstance && !type.HasGenericParameters) return type.Name;
            
            StringBuilder sb = new StringBuilder(type.NameWithoutGenerics());
            List<string> argNames = [];
            if (type is GenericInstanceType generic)
            {
                argNames.AddRange(generic.GenericArguments.Select(t => t.NormalizedFullName()));
            } else argNames = type.GenericParameters.Select(p => p.NormalizedName()).ToList();
            
            sb.Append('<');
            sb.Append(string.Join(", ", argNames));
            sb.Append('>');
            return sb.ToString();
        }

        public string NormalizedFullName()
        {
            if (type.TryGetBuiltInName(out string? builtInName))
            {
                return builtInName;
            }
            
            StringBuilder sb = new StringBuilder();
            if (type.DeclaringType is null)
            {
                sb.Append(type.Namespace);
                if (sb.Length > 0) sb.Append('.');
                sb.Append(type.NormalizedName());
            }
            else
            {
                Stack<TypeReference> stack = new Stack<TypeReference>();
                TypeReference? current = type;
                while (current is not null)
                {
                    stack.Push(current);
                    current = current.DeclaringType;
                }
                
                while (stack.Count > 0)
                {
                    TypeReference t = stack.Pop();
                    sb.Append(t.Namespace);
                    if (sb.Length > 0) sb.Append(t.IsNested ? '+' : '.');
                    sb.Append(t.NormalizedName());
                }
            }
            
            return sb.ToString();
        }

        public IMemberDefinition GetMember(string name, Type memberType)
        {
            if (memberType == typeof(PropertyDefinition))
            {
                return type.GetProperty(name) ?? throw new ArgumentException($"Property '{name}' not found in type '{type.FullName}'");
            }

            if (memberType == typeof(FieldDefinition))
            {
                return type.GetField(name) ?? throw new ArgumentException($"Field '{name}' not found in type '{type.FullName}'");
            }

            if (memberType == typeof(MethodDefinition))
            {
                return type.GetMethod(name) ?? throw new ArgumentException($"Method '{name}' not found in type '{type.FullName}'");
            }

            throw new ArgumentException($"Unsupported member type: {memberType}");
        }

        public PropertyDefinition? GetProperty(string name)
        {
            return !type.Resolve().HasProperties ? null : type.Resolve().Properties.FirstOrDefault(property => property.Name == name);
        }

        public FieldDefinition? GetField(string name)
        {
            return !type.Resolve().HasFields ? null : type.Resolve().Fields.FirstOrDefault(field => field.Name == name);
        }

        public MethodDefinition? GetMethod(string name)
        {
            return !type.Resolve().HasMethods ? null : type.Resolve().Methods.FirstOrDefault(method => method.Name == name);
        }

        public List<TypeDefinition> GetNestedTypes()
        {
            if (!type.Resolve().HasNestedTypes) return [];
        
            List<TypeDefinition> nestedTypes = [];
            foreach (var nested in type.Resolve().NestedTypes)
            {
                nestedTypes.Add(nested);
                nestedTypes.AddRange(nested.GetNestedTypes());
            }
            return nestedTypes;
        }

        public TypeDefinition? GetType(string name, bool fullName = true)
        {
            if (!type.Resolve().HasNestedTypes) return null;
        
            foreach (var nested in type.Resolve().NestedTypes)
            {
                if (fullName && nested.FullName == name || !fullName && nested.Name == name) return nested;

                try
                {
                    TypeDefinition? found = nested.GetType(name, fullName);
                    if (found is not null) return found;
                }
                catch (ArgumentException)
                {
                    // Ignore and continue searching
                }
            }
        
            return null;
        }
    }
}