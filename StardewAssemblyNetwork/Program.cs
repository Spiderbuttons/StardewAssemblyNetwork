using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;

[assembly: InternalsVisibleTo("StardewAssemblyNetwork.Test")]

namespace StardewAssemblyNetwork;

class Program
{
    public const string GAME_FOLDER = @"C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley";

    public static AssemblyManager AssemblyManager { get; private set; } = null!;
    
    static void Main(string[] args)
    {
        AssemblyManager = new AssemblyManager(new DefaultAssemblyResolver());
        DirectoryInfo gameFolder = new DirectoryInfo(GAME_FOLDER);

        HashSet<string> foldersWithDLLs = [];
        foreach (var file in gameFolder.GetFiles("*.dll", SearchOption.AllDirectories))
        {
            if (file.Directory is not null)
            {
                AssemblyManager.TryAddSearchDirectory(file.Directory.FullName);
            }
        }
        
        // FileInfo? stardewAssembly = gameFolder.GetFiles("BETAS.dll", SearchOption.AllDirectories).FirstOrDefault();
        // if (stardewAssembly == null)
        // {
        //     Console.WriteLine("Stardew Valley.dll not found in the game folder.");
        //     return;
        // }
        
        AssemblyDefinition assembly = AssemblyManager.GetAssembly("StardewAssemblyNetwork.TestAssembly.dll");
        // TODO: Will need to remember to add all the folders with .dll mods in them as search directories.
        foreach (var folder in foldersWithDLLs) 
        {
            (assembly.MainModule.AssemblyResolver as DefaultAssemblyResolver)?.AddSearchDirectory(folder);
        }
        
        // (assembly.MainModule.AssemblyResolver as DefaultAssemblyResolver)?.AddSearchDirectory(gameFolder.FullName);
        // (assembly.MainModule.AssemblyResolver as DefaultAssemblyResolver)?.AddSearchDirectory(Path.Combine(GAME_FOLDER, "smapi-internal"));
        
        foreach (var reference in assembly.MainModule.AssemblyReferences)
        {
            // AssemblyDefinition? referencedAssembly = assembly.MainModule.AssemblyResolver.Resolve(reference);
            // if (referencedAssembly == null)
            // {
            //     Console.WriteLine($"Referenced assembly {reference.FullName} not found.");
            //     continue;
            // }
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
                // Console.WriteLine($"{nested.FullNameNormalized()}");
                
                foreach (var member in nested.Methods)
                {
                    if (member.CustomAttributes.Any(attr => attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute")) continue;
                    // Console.WriteLine($"\t{nested.FullNameNormalized()}.{member.NameNormalized()}");
                }
            }
            
            if (type.Name is "<Module>") continue;
            // if (type.IsCompilerGenerated()) continue;
            if (type.Namespace == "System.Runtime.CompilerServices") continue;
            
            // if (!type.HasGenericParameters) continue;

            // foreach (var genParam in type.GenericParameters)
            // {
            //     Console.WriteLine(genParam.Name);
            // }
            
            // Console.WriteLine($"{type.FullNameNormalized()}");
            
            foreach (var member in type.Methods)
            {
                memberCount++;
                if (!member.Name.Contains("Dummy")) continue;
                // if (member.IsCompilerGenerated()) continue;
                
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
                // Console.WriteLine($"\t{type.FullNameNormalized()}.{member.NameNormalized()}");
                // Console.WriteLine($"\t\t{member.FullName}");
            }
        }
        
        Console.WriteLine(memberCount);
    }
}