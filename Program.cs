using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
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

        int memberCount = 0;
        foreach (var type in assembly.MainModule.Types)
        {
            memberCount++;
            if (!type.Name.Contains("BETAS")) continue;
            
            // TODO: Definitely find a better way to do this. Need to recursively find types.
            foreach (var nested in type.NestedTypes)
            {
                if (nested.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
                Console.WriteLine($"{nested.FullNameNormalized()}");
                
                foreach (var member in nested.Methods)
                {
                    if (member.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
                    Console.WriteLine($"\t{nested.FullNameNormalized()}.{member.NameNormalized()}");
                }
            }
            
            if (type.Name is "<Module>") continue;
            if (type.IsCompilerGenerated()) continue;
            if (type.Namespace == "System.Runtime.CompilerServices") continue;
            
            // if (!type.HasGenericParameters) continue;

            // foreach (var genParam in type.GenericParameters)
            // {
            //     Console.WriteLine(genParam.Name);
            // }
            
            Console.WriteLine($"{type.FullNameNormalized()}");
            
            foreach (var member in type.Methods)
            {
                memberCount++;
                if (!member.Name.Contains("Dummy")) continue;
                if (member.IsCompilerGenerated()) continue;
                
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
                Console.WriteLine($"\t{type.FullNameNormalized()}.{member.NameNormalized()}");
                // Console.WriteLine($"\t\t{member.FullName}");
            }
        }
        
        Console.WriteLine(memberCount);
    }
}

public static class MemberExtensions
{
    public static string ParsedGenericName<T>(this T member) where T : MemberReference, IGenericParameterProvider
    {
        if (member is IGenericInstance { HasGenericArguments: true } generic)
        {
            // if (member.FullName.StartsWith("System.Nullable")) return $"{generic.GenericArguments[0].NameNormalized()}?";
            
            string genericArgs = string.Join(", ", generic.GenericArguments.Select(arg => arg.FullNameNormalized()));
            return $"{member.FullName.Split('`')[0]}<{genericArgs}>";
        }
        
        if (!member.HasGenericParameters) return member.Name;
     
        // TODO: Eventually I wanna see how much space is saved in the final json by removing the space in the delimiter.
        string genericParams = string.Join(", ", member.GenericParameters.Select(p => p.Name));
        return $"{member.Name.Split('`')[0]}<{genericParams}>";
    }
    
    public static bool TryParseBuiltInTypeName(MemberReference member, [NotNullWhen(true)] out string? builtInType)
    {
        if (member.FullName.StartsWith("System.Nullable") && member is GenericInstanceType { HasGenericArguments: true } generic)
        {
            builtInType = $"{generic.GenericArguments[0].FullNameNormalized()}"; // TODO: Re-add the "?" I removed here.
            return true;
        }
        
        builtInType = member.FullName switch
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
            _ => null
        };
        return builtInType is not null;
    }
    
    public static bool IsNullable(this Mono.Cecil.ICustomAttributeProvider provider)
    {
        return false; // This needs more work before I wanna start using it.
        return provider.HasCustomAttributes && provider.CustomAttributes.Any(attr =>
            attr.AttributeType.FullName is "System.Runtime.CompilerServices.NullableAttribute");
    }
    
    public static bool IsDynamic(this Mono.Cecil.ICustomAttributeProvider provider)
    {
        return provider.HasCustomAttributes && provider.CustomAttributes.Any(attr => attr.AttributeType.FullName is "System.Runtime.CompilerServices.DynamicAttribute");
    }
    
    public static bool IsByRef(this ParameterDefinition parameter)
    {
        return parameter.ParameterType.IsByReference && parameter is { IsIn: false, IsOut: false };
    }

    public static bool IsArray(this ParameterDefinition parameter)
    {
        // You'd think IsArray would be enough but ref array types apparently don't get IsArray = true.
        return parameter.ParameterType.IsArray || parameter.ParameterType is ByReferenceType { ElementType: ArrayType };
    }
    
    public static bool IsCompilerGenerated(this Mono.Cecil.ICustomAttributeProvider provider)
    {
        return provider.HasCustomAttributes && provider.CustomAttributes.Any(attr => attr.AttributeType.FullName is "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
    }

    public static string NameNormalized(this ParameterDefinition parameter)
    {
        StringBuilder sb = new StringBuilder();
        
        if (parameter.IsByRef()) sb.Append("ref ");
        if (parameter.IsIn) sb.Append("in ");
        if (parameter.IsOut) sb.Append("out ");
        
        if (parameter.IsDynamic()) 
            sb.Append("dynamic");
        else if (TryParseBuiltInTypeName(parameter.ParameterType, out var builtInType))
            sb.Append($"{builtInType}");
        else
            try
            {
                TypeDefinition? type = parameter.ParameterType.Resolve();
                if (type is null) sb.Append(parameter.ParameterType.Name);
                else switch (parameter.ParameterType)
                {
                    case GenericInstanceType genericInstance:
                        sb.Append($"{genericInstance.ParsedGenericName()}");
                        break;
                    default:
                        sb.Append($"{type.FullNameNormalized()}");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to resolve parameter type for {parameter.Name}: {e.Message}");
                sb.Append($"{parameter.ParameterType.FullName}");
            }
        
        if (parameter.IsArray()) sb.Append("[]");
        if (parameter.IsNullable()) sb.Append('?');
        sb.Append($" {parameter.Name}");
        if (parameter.IsOptional) sb.Append($" = {parameter.Constant}");
        
        return sb.ToString();

        // try
        // {
        //     TypeDefinition? type = parameter.ParameterType.Resolve();
        //     arraySuffix = type?.IsArray == true ? "[]" : arraySuffix;
        //     return type is null ?
        //         $"{isIn}{isOut}{isRef}{parameter.ParameterType.Name}{nullable}{arraySuffix} {parameter.Name}{optional}" :
        //         $"{isIn}{isOut}{isRef}{type.FullNameNormalized()}{nullable}{arraySuffix} {parameter.Name}{optional}";
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine($"Failed to resolve parameter type for {parameter.Name}: {e.Message}");
        //     return $"{parameter.ParameterType.FullName}{nullable}{arraySuffix} {parameter.Name}";
        // }
    }
    
    public static string NameNormalized<T>(this T member) where T : MemberReference, IGenericParameterProvider
    {
        if (TryParseBuiltInTypeName(member, out var normalized))
        {
            return normalized;
        }

        normalized = member.ParsedGenericName();

        if (member is not MethodDefinition method) return normalized;
        
        
        string parameters = string.Join(", ", method.Parameters.Select(p =>
        {
            // if (TryParseBuiltInTypeName(p.ParameterType, out var builtInType))
            // {
            //     return $"{builtInType} {p.Name}";
            // }
            return p.NameNormalized();
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
        return normalized;
    }
    
    public static string FullNameNormalized<T>(this T member) where T : MemberReference, IGenericParameterProvider
    {
        if (TryParseBuiltInTypeName(member, out var normalized))
        {
            return normalized;
        }
        
        Stack<TypeReference> parentTypes = new Stack<TypeReference>();
        TypeReference? currentType = member.DeclaringType;
        while (currentType != null)
        {
            parentTypes.Push(currentType);
            currentType = currentType.DeclaringType;
        }
        
        normalized = string.Join(".", parentTypes.Select(t => t.NameNormalized()));
        if (!string.IsNullOrEmpty(normalized) && member.Name is not "set_Item" and not "get_Item") normalized += ".";
        normalized += member.NameNormalized();

        if (member is TypeReference type)
        {
            normalized = type.Namespace + (string.IsNullOrEmpty(type.Namespace) ? "" : ".") + normalized;
        }
        
        return normalized;
    }
}