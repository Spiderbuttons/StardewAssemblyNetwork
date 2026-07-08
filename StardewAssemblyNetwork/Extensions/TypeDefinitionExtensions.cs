using System.Diagnostics.CodeAnalysis;
using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class TypeDefinitionExtensions
{
    extension(TypeDefinition type)
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
                "System.Object" when type.IsDynamic() => "dynamic",
                "System.Object" when !type.IsDynamic() => "object",
                _ => null
            };
            return builtInName is not null;

            if (!type.FullName.StartsWith("System.Nullable")) return false;
            if (!type.HasGenericParameters || type.GenericParameters.Count != 1) return false;
            
            builtInName = $"{type.GenericParameters[0].Name}?";
            return true;
        }

        public string NameWithoutGenerics()
        {
            return !type.HasGenericParameters ? type.Name : type.Name[..type.Name.IndexOf('`')];
        }
        
        public string NormalizedName()
        {
            type.TryGetBuiltInName(out string? builtInName);
            if (!type.HasGenericParameters)
            {
                return builtInName ?? type.Name;
            }
            
            StringBuilder sb = new StringBuilder(type.NameWithoutGenerics());
            IEnumerable<string> genericNames = type.GenericParameters.Select(p => p.Name).ToArray();
            sb.Append('<');
            sb.Append(string.Join(", ", genericNames));
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
                Stack<TypeDefinition> stack = new Stack<TypeDefinition>();
                TypeDefinition? current = type;
                while (current is not null)
                {
                    stack.Push(current);
                    current = current.DeclaringType;
                }
                
                while (stack.Count > 0)
                {
                    TypeDefinition t = stack.Pop();
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
            return !type.HasProperties ? null : type.Properties.FirstOrDefault(property => property.Name == name);
        }

        public FieldDefinition? GetField(string name)
        {
            return !type.HasFields ? null : type.Fields.FirstOrDefault(field => field.Name == name);
        }

        public MethodDefinition? GetMethod(string name)
        {
            return !type.HasMethods ? null : type.Methods.FirstOrDefault(method => method.Name == name);
        }

        public List<TypeDefinition> GetNestedTypes()
        {
            if (!type.HasNestedTypes) return [];
        
            List<TypeDefinition> nestedTypes = [];
            foreach (var nested in type.NestedTypes)
            {
                nestedTypes.Add(nested);
                nestedTypes.AddRange(nested.GetNestedTypes());
            }
            return nestedTypes;
        }

        public TypeDefinition? GetType(string name, bool fullName = true)
        {
            if (!type.HasNestedTypes) return null;
        
            foreach (var nested in type.NestedTypes)
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