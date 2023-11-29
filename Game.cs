using Reloaded.Memory.Pointers;
using TerraFX.Interop.Windows;

namespace SuperCowAPI.SDK;

public static unsafe class Game
{
    public static bool bootMenuActive = false;
    public static unsafe HWND* window = null;
    public static unsafe HWND Window =>*window;
    public static void Init() {
        Memory.Pattern<Ptr<HWND>> movWindow=new Memory.Pattern<Ptr<HWND>>("A3 ? ? ? ? 83 3D ? ? ? ? ? 74 ? 68 ? ? ? ? E8 ? ? ? ?").Search(out var res);
        window = res.AddByteOffset(1).Get();
        Console.WriteLine($"Window pointer is "+new IntPtr(window).ToString("x8"));
        SDK.DirectX.Init();
        var d3d = DirectX.D3dDevice;
        var test = "";
    }
}