using Reloaded.Hooks.Definitions.X86;
using SuperCowAPI.Memory;

namespace SuperCowAPI;

public static unsafe class WinMainEvents
{
    
    [Function(CallingConventions.Stdcall)]
    public delegate int WinMain();

    public static int _WinMain()
    {
        EventManager.Emit<WinMain>();
        var res = WinMainHook.GetTrampoline()();
        return res;
    }

    public static unsafe AutoHookFunction<WinMain> WinMainHook = new(new Pattern<int>("55 8B EC 83 EC ? 68 ? ? ? ? E8 ? ? ? ? 83 C4 ? 85 C0").Search().Pointer, _WinMain);

}