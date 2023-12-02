using System.Runtime.InteropServices;
using PInvoke;
using SuperCowAPI.SDK;
using TerraFX.Interop.Windows;

namespace SuperCowAPI;

public static unsafe class dx_utils
{
    public static unsafe User32.MSG* msg =(User32.MSG*)NativeMemory.Alloc((UIntPtr)Marshal.SizeOf<User32.MSG>());
    public static void force_render_tick() {
        
        SDK.Game.currentTickIsInner = true;
        var pd = DirectX.D3dDevice;
        uint D3DCLEAR_TARGET = 0x00000001;//прост понятнее
            (pd)->Clear(0, null,D3DCLEAR_TARGET , 0xFF121212, 0, 0);
        EventManager.Emit<TickEvents.BeforeTickEvent>();
        (pd)->Present(null,null,HWND.NULL, null );
        EventManager.Emit<TickEvents.AfterTickEvent>();
        SDK.Game.currentTickIsInner = false;
        if ( User32.PeekMessage(msg, IntPtr.Zero, 0, 0, (User32.PeekMessageRemoveFlags)1u) ) {
            User32.TranslateMessage(msg);
            User32.DispatchMessage(msg);
        }

    }
}