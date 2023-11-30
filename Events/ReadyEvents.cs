using Reloaded.Hooks.Definitions.X86;

namespace SuperCowAPI;

public class ReadyEvents
{
    [Function(CallingConventions.Stdcall)]
    public delegate void ReadyEvent();
}