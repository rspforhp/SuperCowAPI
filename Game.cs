using Reloaded.Memory.Pointers;
using SuperCowAPI.Memory;
using TerraFX.Interop.Windows;

namespace SuperCowAPI.SDK;

public static unsafe class Game
{
    public static bool bootMenuActive = false;
    public static unsafe HWND* window = null;
    public static bool currentTickIsInner;
    public static bool booted;
    public static unsafe HWND Window =>*window;
    public static unsafe Vanara.PInvoke.HWND Window_ =>*(Vanara.PInvoke.HWND*)window;
    public static void Init() {
        Memory.Pattern<Ptr<HWND>> movWindow=new Memory.Pattern<Ptr<HWND>>("A3 ? ? ? ? 83 3D ? ? ? ? ? 74 ? 68 ? ? ? ? E8 ? ? ? ?").Search(out var res);
        window = res.AddByteOffset(1).Get();
        Console.WriteLine($"Window pointer is "+new IntPtr(window).ToString("x8"));
        SDK.DirectX.Init();
    }

    public static bool IsGameLoaded()
    {
        Pattern<Ptr<bool>> gameLoadFinishedPat=new ("C6 05 ? ? ? ? ? 83 3D ? ? ? ? ? 0F 84"); // mov     hasGameLoaded, 1
         var mem = gameLoadFinishedPat.Search();
         var ptr = *mem.AddByteOffset(2).Pointer;
        if (ptr == null) return false;
        return *ptr.Pointer;
    }
}