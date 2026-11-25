using System.Runtime.CompilerServices;
using System.Text;

static class TestSetup
{
    [ModuleInitializer]
    public static void Init()
    {
        Console.OutputEncoding = Encoding.UTF8;
    }
}