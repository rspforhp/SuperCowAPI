using System.Diagnostics;
using Iced.Intel;
using PInvoke;
using Reloaded.Hooks;
using Reloaded.Hooks.ReloadedII.Interfaces;
using Reloaded.Memory.Pointers;
using Reloaded.Mod.Interfaces;
using SuperCowAPI.Template;
using SuperCowAPI.Configuration;
using SuperCowAPI.SDK;
using TerraFX.Interop.Gdiplus;

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


        // For more information about this template, please see
        // https://reloaded-project.github.io/Reloaded-II/ModTemplate/

        // If you want to implement e.g. unload support in your mod,
        // and some other neat features, override the methods in ModBase.

        // TODO: Implement some mod logic
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
         Debugger.Launch();

        bool shiftPressed = (User32.GetAsyncKeyState((int)User32.VirtualKey.VK_SHIFT) & 0x01) > 0;


        Config.Init();
        if (Config.GetAndEnsureKey("disabled").GetBool() is true)
        {
            if (!shiftPressed) return true;
            Config.Set("disabled",false);
            Config.Save();
        }

        if (shiftPressed) SDK.Game.bootMenuActive = true;

        init();
        Console.WriteLine("Мод загружен!");
        postInit();

        return true;
    }


    void postInit()
    {
        // ModManager.InitMods();
        // ModFileResolver.Init();
        //  EventManager.Emit(ReadyEvent());
    }

    unsafe void  init()
    {
        var cwd = Directory.GetCurrentDirectory();
        Console.WriteLine("Загрузка SuperMod " + "1.0.0" + " by zziger...");


        GdiplusStartupInput gdiplusStartupInput;
        UIntPtr gdiplusToken;
        Gdiplus.GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, null);

        //MH_Initialize();
        
        SDK.Game.Init();
        /*
        game.AssetPool.Init();

        ModManager.Init();
        hook_start_execution();

        static constexpr Memory.Pattern pat("E8 ? ? ? ? C6 05 ? ? ? ? ? 0F B6 05 ? ? ? ? 85 C0 74 ? 6A");
        HookManager.RegisterHook(Memory(pat.Search().GoToNearCall().Get<void*>()), HOOK_REF(load_game));

        EventManager.On<StartExecutionEvent>([] {
            Log.Info << "Пост-инициализация" << Log.Endl;
            utils.handle_error(postInit, "пост-инициализации");
        });

        EventManager.On<GameLoadedEvent>([] {
            Log.Info << "Игра загружена!" << Log.Endl;

            DragAcceptFiles(*SDK.Game.window, true);
        });


        EventManager.On<WindowEvent>([](auto ev) {
            if (ev.msg == WM_DROPFILES)
            {
                auto drop = (HDROP)(ev.wParam);

                uint32_t nCntFiles = DragQueryFileW(drop, -1, 0, 0);

                for (int j = 0; j < nCntFiles; j++)
                {
                    wchar_t szBuf[MAX_PATH];
                    DragQueryFileW(drop, j, szBuf, sizeof(szBuf));
                    std.optional<std.filesystem.path> temp;
                    try
                    {
                        auto path = std.filesystem.path(szBuf);
                        if (path.extension() == ".zip")
                        {
                            miniz_cpp.zip_file zip {
                                path.string()
                            }
                            ;
                            auto list = zip.namelist();
                            auto manifestRes = std.ranges.find_if(list, [](std.string str){
                                return str.ends_with("manifest.yml");
                            });
                            if (manifestRes == list.end())
                            {
                                MessageBoxA(nullptr, "Неверный архив модв!", nullptr, MB_OK | MB_ICONERROR);
                                continue;
                            }

                            auto root = (*manifestRes).substr(0, (*manifestRes).size() - 12); // manifest.yml = 12 chars
                            Log.Debug << "Found mod in " << path << " at \"" << root << "\"" << Log.Endl;
                            auto info = ModInfo(zip.read(root + "manifest.yml"));
                            info.zipFile = path;
                            info.zipRoot = root;
                            ModManager.RequestModInstall(info);
                        }
                        else
                        {
                            ModManager.RequestModInstall(ModInfo(path));
                        }
                    }
                    catch (std.exception

                    &e) {
                        Log.Error << "Ошибка установки мода: " << e.what() << Log.Endl;
                    } catch(...) {
                        Log.Error << "Неизвестная ошибка установки мода" << Log.Endl;
                    }
                }

                if (nCntFiles > 0) SetForegroundWindow(*SDK.Game.window);

                DragFinish(drop);
            }
        });
        */
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
                typeof(ModBase), typeof(Ptr<>), typeof(Assembler), typeof(Function<>),
            };
            return ar.ToArray();
        }
    }
}