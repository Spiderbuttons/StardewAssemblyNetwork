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
            Console.WriteLine($"{type.FullName}");
        }
    }
}