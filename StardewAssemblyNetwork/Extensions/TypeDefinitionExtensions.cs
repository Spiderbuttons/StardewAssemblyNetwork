using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class TypeDefinitionExtensions
{
    extension(TypeDefinition type)
    {
        public string NormalizedName()
        {
            if (!type.HasGenericParameters) return type.Name;
            
            StringBuilder sb = new StringBuilder(type.Name[..type.Name.IndexOf('`')]);
            IEnumerable<string> genericNames = type.GenericParameters.Select(p => p.Name).ToArray();
            sb.Append('<');
            sb.Append(string.Join(", ", genericNames));
            sb.Append('>');
            return sb.ToString();
        }

        public string NormalizedFullName()
        {
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