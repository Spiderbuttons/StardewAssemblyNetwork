using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Mono.Cecil;

namespace StardewAssemblyNetwork;

class Program
{
    public const string GAME_FOLDER = @"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley";
    
    static void Main(string[] args)
    {
        DirectoryInfo gameFolder = new DirectoryInfo(GAME_FOLDER);
        FileInfo? stardewAssembly = gameFolder.GetFiles("Stardew Valley.dll", SearchOption.AllDirectories).FirstOrDefault();
        if (stardewAssembly == null)
        {
            Console.WriteLine("Stardew Valley.dll not found in the game folder.");
            return;
        }
        
        AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(stardewAssembly.FullName);

        foreach (var type in assembly.MainModule.Types)
        {
            // if (type.Name is not "Bimap`2") continue;
            if (type.Name.Contains("OverlayDictionary") == false) continue;
            
            if (type.Name is "<Module>") continue;
            if (type.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
            
            if (!type.HasGenericParameters) continue;

            // foreach (var genParam in type.GenericParameters)
            // {
            //     Console.WriteLine(genParam.Name);
            // }
            
            Console.WriteLine($"{type.FullNameNormalized()}");
            
            foreach (var member in type.Methods)
            {
                // if (member.Name is not "CopyTo") continue;
                if (member.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
                
                Console.WriteLine($"  {type.FullNameNormalized()}.{member.NameNormalized()}");
            }
        }
    }
}

public static class MemberExtensions
{
    public static string NameNormalized<T>(this T member) where T : IMemberDefinition, IGenericParameterProvider
    {
        string normalized = member.Name;

        if (member.HasGenericParameters)
        {
            // TODO: Eventually I wanna see how much space is saved in the final json by removing the space in the delimiter.
            string genericParams = string.Join(", ", member.GenericParameters.Select(p => p.Name));
            normalized = $"{member.Name.Split('`')[0]}<{genericParams}>";
        }

        if (member is MethodDefinition method)
        {
            string parameters = string.Join(", ", method.Parameters.Select(p =>
            {
                TypeDefinition? type = p.ParameterType.Resolve();
                return type is null ? $"{p.ParameterType.Name} {p.Name}" : $"{type.FullNameNormalized()} {p.Name}";
            }));

            if (method.IsSpecialName)
            {
                if (method.IsConstructor)
                {
                    normalized = $"{member.DeclaringType.NameNormalized()}({parameters})";
                } else if (member.Name is "set_Item" or "get_Item")
                {
                    normalized = $"[]";
                } else if (member.Name.StartsWith("get_"))
                {
                    normalized = $"{member.Name.Substring(4)} {{ get; }}";
                } else if (member.Name.StartsWith("set_"))
                {
                    normalized = $"{member.Name.Substring(4)} {{ set; }}";
                }
            } else normalized += $"({parameters})";
        }
        return normalized;
    }
    
    public static string FullNameNormalized<T>(this T member) where T : IMemberDefinition, IGenericParameterProvider
    {
        Stack<TypeDefinition> parentTypes = new Stack<TypeDefinition>();
        TypeDefinition? currentType = member.DeclaringType;
        while (currentType != null)
        {
            parentTypes.Push(currentType);
            currentType = currentType.DeclaringType;
        }
        
        string fullName = string.Join(".", parentTypes.Select(t => t.NameNormalized()));
        if (!string.IsNullOrEmpty(fullName) && member.Name is not "set_Item" and not "get_Item") fullName += ".";
        fullName += member.NameNormalized();

        if (member is TypeReference type)
        {
            fullName = type.Namespace + (string.IsNullOrEmpty(type.Namespace) ? "" : ".") + fullName;
        }
        
        return fullName;
    }
}