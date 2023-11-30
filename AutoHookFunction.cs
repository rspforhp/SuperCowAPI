using Reloaded.Mod.Interfaces;

namespace SuperCowAPI;
using System.Runtime.CompilerServices;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions;

public static class StaticMod
{
    public static List<IHook> Hooks = new();


    /// <summary>
    ///     Provides access to the mod loader API.
    /// </summary>
    public static IModLoader _modLoader;

    /// <summary>
    ///     Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    public static IReloadedHooks _hooks;

    /// <summary>
    ///     Provides access to the Reloaded logger.
    /// </summary>
    public static ILogger _logger;

    /// <summary>
    ///     Entry point into the mod, instance that created this class.
    /// </summary>
    public static IMod _owner;


    /// <summary>
    ///     The configuration of the currently executing mod.
    /// </summary>
    public static IModConfig _modConfig;
}
public class AutoHookFunction<T> : Function<T> where T : Delegate
{
    public T Trampoline;

    public  void CreateHook(T hook) 
    {
        var a = Hook(hook);
        a.Activate();
        StaticMod.Hooks.Add(a);
        Trampoline = new Function<T>((UIntPtr)a.OriginalFunctionAddress, StaticMod._hooks).GetWrapper();
    }

    public AutoHookFunction(int address,  T hook) : base((UIntPtr)address, StaticMod._hooks)
    {
        this.CreateHook(hook);
    }

    public AutoHookFunction(UIntPtr address,  T hook) : base(address, StaticMod._hooks)
    {
        this.CreateHook(hook);
    }
    public unsafe AutoHookFunction(int* address,  T hook) : base((UIntPtr)address, StaticMod._hooks)
    {
        this.CreateHook(hook);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public T GetTrampoline()
    {
        return Trampoline;
    }
}