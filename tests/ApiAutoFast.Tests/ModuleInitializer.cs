using System.Runtime.CompilerServices;
using VerifyTests;

namespace ApiAutoFast.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
