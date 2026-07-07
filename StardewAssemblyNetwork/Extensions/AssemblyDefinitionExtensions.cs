using Mono.Cecil;

namespace StardewAssemblyNetwork.Extensions;

public static class AssemblyDefinitionExtensions
{
    extension(AssemblyDefinition assembly)
    {
        public List<TypeDefinition> GetAllTypes()
        {
            if (!assembly.MainModule.HasTypes) return [];
        
            List<TypeDefinition> allTypes = [];
            foreach (var type in assembly.MainModule.Types)
            {
                allTypes.Add(type);
                allTypes.AddRange(type.GetNestedTypes());
            }
            return allTypes;
        }

        public TypeDefinition GetType(string name, bool fullName = true)
        {
            foreach (var type in assembly.MainModule.Types)
            {
                if (fullName && type.FullName == name || !fullName && type.Name == name) return type;

                try
                {
                    TypeDefinition? found = type.GetType(name, fullName);
                    if (found is not null) return found;
                }
                catch (ArgumentException)
                {
                    // Ignore and continue searching
                }
            }
        
            throw new ArgumentException($"Type '{name}' not found in assembly '{assembly.FullName}'");
        }
    }
}