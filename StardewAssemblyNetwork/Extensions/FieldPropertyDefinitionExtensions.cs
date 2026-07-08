using System.Text;
using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class FieldPropertyDefinitionExtensions
{
    extension(PropertyDefinition prop)
    {
        public string NormalizedName()
        {
            if (prop.IsIndexer())
            {
                return $"Indexer[{string.Join(", ", prop.Parameters.Select(p => p.NormalizedFullName(prop)))}]";
            }
            
            StringBuilder sb = new StringBuilder(prop.Name);
            sb.Append(" { ");
            if (prop.GetMethod is not null) sb.Append("get; ");
            if (prop.SetMethod is not null) sb.Append("set; ");
            sb.Append('}');
            return sb.ToString();
        }
        
        public string NormalizedFullName()
        {
            if (prop.IsIndexer())
            {
                return $"{prop.DeclaringType?.NormalizedFullName()}::Indexer[{string.Join(", ", prop.Parameters.Select(p => p.NormalizedFullName(prop)))}]";
            }
            
            return $"{prop.DeclaringType?.NormalizedFullName()}.{prop.NormalizedName()}";
        }

        public bool IsIndexer()
        {
            TypeDefinition? declaringType = prop.DeclaringType;
            if (declaringType == null || !declaringType.TryGetAttribute("DefaultMemberAttribute", out var attr)) return prop.HasParameters;

            if (!attr.HasConstructorArguments) return false;
            
            string? defaultMemberName = attr.ConstructorArguments[0].Value as string;
            return defaultMemberName == prop.Name;
        }
    }
    
    extension(FieldDefinition field)
    {
        public string NormalizedName()
        {
            return field.Name;
        }
        
        public string NormalizedFullName()
        {
            return $"{field.DeclaringType?.NormalizedFullName()}.{field.NormalizedName()}";
        }
    }
}