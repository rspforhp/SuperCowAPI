using Reloaded.Hooks.Definitions.X86;

namespace SuperCowAPI;

public static unsafe class ReadyEvents
{
    [Function(CallingConventions.Stdcall)]
    public delegate void ReadyEvent();
}