using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;

namespace StardewAssemblyNetwork;

public class AssemblyManager
{
    public DefaultAssemblyResolver AssemblyResolver { get; set; }
    private HashSet<string> SearchDirectories { get; set; } = [];

    public AssemblyManager(DefaultAssemblyResolver? assemblyResolver)
    {
        AssemblyResolver = assemblyResolver ?? new DefaultAssemblyResolver();
        TryAddSearchDirectory(Directory.GetCurrentDirectory());
    }

    public AssemblyDefinition GetAssembly(string filePath)
    {
        try
        {
            return AssemblyDefinition.ReadAssembly(filePath, new ReaderParameters { AssemblyResolver = AssemblyResolver });
        }
        catch (Exception e)
        {
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), filePath)))
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                return AssemblyDefinition.ReadAssembly(path, new ReaderParameters { AssemblyResolver = AssemblyResolver });
            }
            
            foreach (var dir in AssemblyResolver.GetSearchDirectories())
            {
                if (!File.Exists(Path.Combine(dir, filePath))) continue;
                
                string path = Path.Combine(dir, filePath);
                return AssemblyDefinition.ReadAssembly(path, new ReaderParameters { AssemblyResolver = AssemblyResolver });
            }

            throw new ArgumentException($"Could not find assembly '{filePath}'", e);
        }
    }

    public bool TryAddSearchDirectory(string? directory)
    {
        if (directory is null || !SearchDirectories.Add(directory) || !Directory.Exists(directory))
            return false;

        AssemblyResolver.AddSearchDirectory(directory);
        return true;
    }

    public bool TryRemoveSearchDirectory(string? directory)
    {
        if (directory is null || !SearchDirectories.Remove(directory))
            return false;
        
        AssemblyResolver.RemoveSearchDirectory(directory);
        return true;
    }
}