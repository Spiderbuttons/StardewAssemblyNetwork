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

        HashSet<string> foldersWithDLLs = [];
        foreach (var file in gameFolder.GetFiles("*.dll", SearchOption.AllDirectories))
        {
            if (file.Directory is not null)
            {
                foldersWithDLLs.Add(file.Directory.FullName);
            }
        }
        
        FileInfo? stardewAssembly = gameFolder.GetFiles("BETAS.dll", SearchOption.AllDirectories).FirstOrDefault();
        if (stardewAssembly == null)
        {
            Console.WriteLine("Stardew Valley.dll not found in the game folder.");
            return;
        }
        
        AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(stardewAssembly.FullName);
        // TODO: Will need to remember to add all the folders with .dll mods in them as search directories.
        foreach (var folder in foldersWithDLLs) 
        {
            (assembly.MainModule.AssemblyResolver as DefaultAssemblyResolver)?.AddSearchDirectory(folder);
        }
        
        // (assembly.MainModule.AssemblyResolver as DefaultAssemblyResolver)?.AddSearchDirectory(gameFolder.FullName);
        // (assembly.MainModule.AssemblyResolver as DefaultAssemblyResolver)?.AddSearchDirectory(Path.Combine(GAME_FOLDER, "smapi-internal"));
        
        foreach (var reference in assembly.MainModule.AssemblyReferences)
        {
            AssemblyDefinition? referencedAssembly = assembly.MainModule.AssemblyResolver.Resolve(reference);
            if (referencedAssembly == null)
            {
                Console.WriteLine($"Referenced assembly {reference.FullName} not found.");
                continue;
            }
            // Console.WriteLine($"Referenced assembly: {referencedAssembly.FullName}");
        }

        foreach (var type in assembly.MainModule.Types)
        {
            // if (type.Name is not "BETAS") continue;
            
            // TODO: Definitely find a better way to do this. Need to recursively find types.
            foreach (var nested in type.NestedTypes)
            {
                if (nested.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
                Console.WriteLine($"{nested.FullNameNormalized()}");
                
                foreach (var member in nested.Methods)
                {
                    if (member.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
                    Console.WriteLine($"  {nested.FullNameNormalized()}.{member.NameNormalized()}");
                }
            }
            
            if (type.Name is "<Module>") continue;
            if (type.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
            if (type.Namespace == "System.Runtime.CompilerServices") continue;
            
            // if (!type.HasGenericParameters) continue;

            // foreach (var genParam in type.GenericParameters)
            // {
            //     Console.WriteLine(genParam.Name);
            // }
            
            Console.WriteLine($"{type.FullNameNormalized()}");
            
            foreach (var member in type.Methods)
            {
                // if (member.Name is not "DummyFunction") continue;
                if (member.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
                
                // var param = member.Parameters[3];
                // foreach (var propInfo in param.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                // {
                //     var value = propInfo.GetValue(param);
                //     if (value is not Mono.Collections.Generic.Collection<CustomAttribute> collection)
                //     {
                //         Console.WriteLine($"    {propInfo.Name}: {value}");
                //         continue;
                //     } else 
                //     {
                //         Console.WriteLine($"    {propInfo.Name}:");
                //         foreach (var item in collection)
                //         {
                //             Console.WriteLine($"      {item.AttributeType.FullName}");
                //         }
                //     }
                // }
                //
                Console.WriteLine($"  {type.FullNameNormalized()}.{member.NameNormalized()}");
            }
        }
    }
}

public static class MemberExtensions
{
    public static string NameNormalized<T>(this T member) where T : IMemberDefinition, IGenericParameterProvider
    {
        string normalized = member.FullName switch
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
            "System.Object" => "object",
            "System.Delegate" => "delegate",
            _ => string.Empty
        };
        if (!string.IsNullOrEmpty(normalized)) return normalized;
        normalized = member.Name;

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
                string nullable = p.CustomAttributes.Any(attr => attr.AttributeType.FullName is "System.Runtime.CompilerServices.NullableAttribute") ? "?" : "";
                
                bool dynamic = p.CustomAttributes.Any(attr => attr.AttributeType.FullName is "System.Runtime.CompilerServices.DynamicAttribute");
                
                string isIn = p.IsIn ? "in " : "";
                string isOut = p.IsOut ? "out " : "";
                string optional = p.IsOptional ? $" = {p.Constant}" : "";
                
                string arraySuffix = p.ParameterType.IsArray || p.ParameterType.Name.Contains("[]") ? "[]" : "";
                string isRef = p.ParameterType.IsByReference && string.IsNullOrEmpty(isOut) && string.IsNullOrEmpty(isIn) ? "ref " : "";
                
                if (dynamic)
                {
                    return $"dynamic{nullable} {p.Name}";
                }
                
                try
                {
                    TypeDefinition? type = p.ParameterType.Resolve();
                    arraySuffix = type?.IsArray == true ? "[]" : arraySuffix;
                    return type is null ? 
                        $"{isIn}{isOut}{isRef}{p.ParameterType.Name}{nullable}{arraySuffix} {p.Name}{optional}" : 
                        $"{isIn}{isOut}{isRef}{type.FullNameNormalized()}{nullable}{arraySuffix} {p.Name}{optional}";
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to resolve parameter type for {p.Name} in method {method.FullNameNormalized()}: {e.Message}");
                    return $"{p.ParameterType.FullName}{nullable}{arraySuffix} {p.Name}";
                }
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
        string fullName = member.FullName switch 
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
            "System.Object" => "object",
            "System.Delegate" => "delegate",
            _ => string.Empty
        };
        if (!string.IsNullOrEmpty(fullName)) return fullName;
        
        Stack<TypeDefinition> parentTypes = new Stack<TypeDefinition>();
        TypeDefinition? currentType = member.DeclaringType;
        while (currentType != null)
        {
            parentTypes.Push(currentType);
            currentType = currentType.DeclaringType;
        }
        
        fullName = string.Join(".", parentTypes.Select(t => t.NameNormalized()));
        if (!string.IsNullOrEmpty(fullName) && member.Name is not "set_Item" and not "get_Item") fullName += ".";
        fullName += member.NameNormalized();

        if (member is TypeReference type)
        {
            fullName = type.Namespace + (string.IsNullOrEmpty(type.Namespace) ? "" : ".") + fullName;
        }
        
        return fullName;
    }
}