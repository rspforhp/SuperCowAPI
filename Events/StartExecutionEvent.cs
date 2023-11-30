using Reloaded.Hooks.Definitions.X86;
using SuperCowAPI.Memory;

namespace SuperCowAPI;

public static unsafe class StartExecutionEvents
{
    public delegate void StartExecutionEvent();


    [Function(CallingConventions.Cdecl)]
    public delegate int can_start_game(CharPointerString lpName);

    public static int can_start_gameHook(CharPointerString lpName)
    {
        EventManager.Emit<StartExecutionEvent>();
        return 1;
    }

    public static AutoHookFunction<can_start_game> can_start_gameHook_ = new(new Pattern<int>("55 8B EC 51 8B 45 ? 50 6A ? 6A ? FF 15").Search().Pointer, can_start_gameHook);


}