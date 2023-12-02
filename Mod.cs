using System.Diagnostics;
using System.Runtime.CompilerServices;
using Iced.Intel;
using PInvoke;
using Reloaded.Hooks;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.Pointers;
using Reloaded.Mod.Interfaces;
using Silk.NET.Direct3D9;
using SuperCowAPI.Template;
using SuperCowAPI.Configuration;
using SuperCowAPI.Memory;
using SuperCowAPI.SDK;
using TerraFX.Interop.Gdiplus;
using TerraFX.Interop.Windows;
using static PInvoke.User32;

namespace SuperCowAPI;

/// <summary>
/// Your mod logic goes here.
/// </summary>
public class Mod : ModBase // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    private readonly IReloadedHooks? _hooks;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// Provides access to this mod's configuration.
    /// </summary>
    private SuperCowAPI.Configuration.Config _configuration;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;
        StaticMod._modLoader = context.ModLoader;
        StaticMod._hooks = context.Hooks;
        StaticMod._logger = context.Logger;
        StaticMod._owner = context.Owner;
        StaticMod._modConfig = context.ModConfig;

        Debugger.Launch();


        InitStaticPatches();

        Main();
    }


    public unsafe bool Main()
    {
        if (Directory.GetCurrentDirectory().Any(a => !char.IsAscii(a)))
        {
            User32.MessageBox(IntPtr.Zero, "Путь к игре не может содержать буквы русского алфавита", "SuperMod",
                User32.MessageBoxOptions.MB_OK);
            Environment.Exit(0);
        }


        bool shiftPressed = (User32.GetAsyncKeyState((int)User32.VirtualKey.VK_SHIFT) & 0x01) > 0;


        bool isDebug = false;


        Config.Init();
        if (Config.GetAndEnsureKey("disabled").GetBool() is true)
        {
            if (!shiftPressed) return true;
            Config.Set("disabled", false);
            Config.Save();
        }

        if (shiftPressed || isDebug) SDK.Game.bootMenuActive = true;

        init();
        Console.WriteLine("Мод загружен!");

        EventManager.On<StartExecutionEvents.StartExecutionEvent>(delegate()
        {
            Console.WriteLine("StartExecutionEvent!");
        });
        return true;
    }


    void postInit()
    {
        // ModManager.InitMods();
        // ModFileResolver.Init();
        EventManager.Emit<ReadyEvents.ReadyEvent>();
    }

    [Function(CallingConventions.Stdcall)]
    public delegate int load_game_();

    public static int load_game()
    {
        Console.WriteLine("Инициализация загрузки игры");

        if ((User32.GetAsyncKeyState((int)User32.VirtualKey.VK_SHIFT) & 0x01) > 0)
        {
            SDK.Game.bootMenuActive = true;
        }

        if (SDK.Game.bootMenuActive)
        {
            Console.WriteLine("Boot меню активно");
            Vanara.PInvoke.User32.EnableMenuItem(GetSystemMenu(SDK.Game.Window, false), (uint)SysCommands.SC_CLOSE,
                Vanara.PInvoke.User32.MenuFlags.MF_BYCOMMAND | Vanara.PInvoke.User32.MenuFlags.MF_DISABLED |
                Vanara.PInvoke.User32.MenuFlags.MF_GRAYED);
        }

        while (SDK.Game.bootMenuActive)
        {
            var start = Kernel32.GetTickCount64();
            dx_utils.force_render_tick();
            var delta = Kernel32.GetTickCount64() - start;
            int needed = 10;


            if (delta < (ulong)needed) Thread.Sleep(needed - (int)delta);
        }

        Vanara.PInvoke.User32.EnableMenuItem(GetSystemMenu(SDK.Game.Window, false), (uint)SysCommands.SC_CLOSE,
            Vanara.PInvoke.User32.MenuFlags.MF_BYCOMMAND | Vanara.PInvoke.User32.MenuFlags.MF_ENABLED);
        SDK.Game.booted = true;
        EventManager.Emit<WindowReadyEvents.WindowReadyEvent>();
        return LoadGameHook.GetTrampoline()();
    }

    public static unsafe AutoHookFunction<load_game_> LoadGameHook = new AutoHookFunction<load_game_>(
        new Pattern<int>("E8 ? ? ? ? C6 05 ? ? ? ? ? 0F B6 05 ? ? ? ? 85 C0 74 ? 6A").Search().GoToNearCall()
            .Pointer, load_game);

    unsafe void init()
    {
        var cwd = Directory.GetCurrentDirectory();
        Console.WriteLine("Загрузка SuperMod " + "1.0.0" + " by zziger...");


        GdiplusStartupInput gdiplusStartupInput;
        UIntPtr gdiplusToken;
        Gdiplus.GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, null);

        //MH_Initialize();

        SDK.Game.Init();

        Game.AssetPool.Init();


        EventManager.On<StartExecutionEvents.StartExecutionEvent>(delegate
        {
            Console.WriteLine("Пост-инициализация");
            postInit();
        });


        EventManager.On<GameLoadedEvents.GameLoadedEvent>(delegate() { Console.WriteLine("Игра загружена!"); });
    }
/*
      */

//This is needed to make sure all autohookfunction's constructors run 
    private static void InitStaticPatches()
    {
        foreach (var type in typeof(Mod).Assembly.GetTypes())
        foreach (var fieldInfo in type.GetFields())
            if (fieldInfo.FieldType.IsGenericType &&
                fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(AutoHookFunction<>))
            {
                Console.WriteLine(type + "  " + fieldInfo.Name);
                var o = fieldInfo.GetValue(null);
                if (o == null) continue;
                o.GetType().GetMethod("GetTrampoline").Invoke(o, null);
            }
    }

    #region Standard Overrides

    public override void ConfigurationUpdated(SuperCowAPI.Configuration.Config configuration)
    {
// Apply settings from configuration.
// ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }

    #endregion

    #region For Exports, Serialization etc.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod()
    {
    }
#pragma warning restore CS8618

    #endregion

    public class Exports : IExports
    {
// Sharing a type actually exports the whole library.  
// So you only really need to share 1 type to export your whole interfaces library.  

        public Type[] GetTypes()
        {
            var ar = new System.Collections.Generic.List<Type>
            {
                typeof(ModBase), typeof(Ptr<>), typeof(Assembler), typeof(Function<>), typeof(AutoHookFunction<>),
                typeof(StaticMod), typeof(IDirect3DDevice9), typeof(Silk.NET.Core.Bool8), typeof(Silk.NET.DXGI.DXGI),
                typeof(Silk.NET.Maths.Rectangle),  typeof(PInvoke.Shell32),
                typeof(PInvoke.Kernel32), typeof(PInvoke.Ole32), typeof(Vanara.PInvoke.Kernel32),
             typeof(Vanara.PInvoke.ObjectTypeListLevel), typeof(Vanara.PInvoke.User32), typeof(PInvoke.User32),
                typeof(Vanara.PInvoke.SECURITY_INFORMATION), typeof(Vanara.Formatter),
                typeof(TerraFX.Interop.Windows.Windows),typeof(PInvoke.Win32ErrorCode),typeof(Vanara.PInvoke.Gdi32)
            };
            return ar.ToArray();
        }
    }
}