using Mono.Cecil;

[assembly: CaptureConsole]
[assembly: CaptureTrace]

namespace StardewAssemblyNetwork.Tests;

public class TestContext : IDisposable
{
    public AssemblyManager AssemblyManager;
    public AssemblyDefinition TestAssembly;
    
    public TestContext()
    {
        AssemblyManager = new AssemblyManager(null);
        TestAssembly = AssemblyManager.GetAssembly("StardewAssemblyNetwork.TestAssembly.dll");
    }

    public void Dispose()
    {
        TestAssembly = null!;
        AssemblyManager = null!;
    }
}

[CollectionDefinition("TestContext")]
public class TestCollection : ICollectionFixture<TestContext>;