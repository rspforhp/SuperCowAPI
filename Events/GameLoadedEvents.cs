using Reloaded.Hooks.Definitions.X86;
using SuperCowAPI.Memory;

namespace SuperCowAPI;

public static unsafe class GameLoadedEvents
{
    public delegate void GameLoadedEvent();


    [Function(CallingConventions.MicrosoftThiscall)]
    public unsafe delegate void game_loaded(void* this_);
    public static bool sentGameLoaded;

    public static unsafe void tickLoopInnerHook(void* this_)
    {
        tickLoopInner_.GetTrampoline()(this_);
        if (!sentGameLoaded && SDK.Game.IsGameLoaded()) {
            sentGameLoaded = true;
            EventManager.Emit<GameLoadedEvent>();
        }   
    }

    public static unsafe AutoHookFunction<game_loaded> tickLoopInner_ = new(new Pattern<int>("55 8B EC 83 EC ? 89 4D ? D9 05 ? ? ? ? D8 1D").Search().Pointer, tickLoopInnerHook);

}