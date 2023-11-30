using Reloaded.Hooks.Definitions.X86;

namespace SuperCowAPI;

public static class WindowReadyEvents
{
    [Function(CallingConventions.Stdcall)]
    public delegate void WindowReadyEvent();
}