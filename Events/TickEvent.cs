using Reloaded.Hooks.Definitions.X86;
using SuperCowAPI.Memory;

namespace SuperCowAPI;

public static unsafe class TickEvents
{
    [Function(CallingConventions.Stdcall)]
    public delegate void BeforeTickEvent();

    [Function(CallingConventions.Stdcall)]
    public delegate void AfterTickEvent();

    [Function(CallingConventions.MicrosoftThiscall)]
    public delegate int Render(void* this_);

    public static int _RenderHook(void* this_)
    {
        EventManager.Emit<BeforeTickEvent>();
        var res = RenderHook.GetTrampoline()(this_);
        EventManager.Emit<AfterTickEvent>();
        return res;
    }

    public static AutoHookFunction<Render> RenderHook = new(new Pattern<int>("55 8B EC 51 E8 ? ? ? ? 6A ?").Search().Pointer, _RenderHook);



}