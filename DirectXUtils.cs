using System.Runtime.InteropServices;
using PInvoke;
using Silk.NET.Maths;
using SuperCowAPI.SDK;

namespace SuperCowAPI;

public static unsafe class dx_utils
{
    public static unsafe User32.MSG* msg =(User32.MSG*)NativeMemory.Alloc((UIntPtr)Marshal.SizeOf<User32.MSG>());
    public static void force_render_tick() {
        
        SDK.Game.currentTickIsInner = true;
        var p = DirectX.d3dDevice;
        var pd = DirectX.D3dDevice;
        (pd)->Clear(0, null, 0x1, 0xFF121212, 0, 0);
        EventManager.Emit<TickEvents.BeforeTickEvent>();
        (pd)->Present((Box2D<int>*)0,(Box2D<int>*)0, IntPtr.Zero,null );
        EventManager.Emit<TickEvents.AfterTickEvent>();
        SDK.Game.currentTickIsInner = false;
        if ( User32.PeekMessage(msg, IntPtr.Zero, 0, 0, (User32.PeekMessageRemoveFlags)1u) ) {
            User32.TranslateMessage(msg);
            User32.DispatchMessage(msg);
        }

    }
}