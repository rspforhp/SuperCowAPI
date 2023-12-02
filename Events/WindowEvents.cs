using PInvoke;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Memory.Pointers;
using SuperCowAPI.Memory;
using TerraFX.Interop.Windows;
using HWND = Vanara.PInvoke.HWND;

namespace SuperCowAPI;

public static unsafe class WindowEvents
{
    [Function(CallingConventions.Stdcall)]
    public delegate int WindowProc(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);
    public delegate int WindowEvent( Ptr<WindowEventData> evt);

    public struct WindowEventData
    {
        public HWND hWnd;
        public User32.WindowMessage msg; 
        public WPARAM wParam;
        public LPARAM lParam;
        private bool _cancelled;

        public void Cancel() {
            _cancelled = true;
        }

        public bool IsCancelled()  {
            return _cancelled;
        }
    }
    public static int wndproc( HWND hWnd,  uint msg,  WPARAM wParam,  LPARAM lParam) {
        WindowEventData evt=new(){hWnd=hWnd,msg=(User32.WindowMessage)msg,wParam=wParam,lParam=lParam};
        EventManager.Emit<WindowEvent>(new Ptr<WindowEventData>(&evt));

        if (evt.IsCancelled()) 
            return (int)User32.DefWindowProc(hWnd.DangerousGetHandle(), (User32.WindowMessage)msg, (IntPtr)wParam, lParam);
    
        return wndproc_.GetTrampoline()(hWnd, msg, wParam, lParam);
    }

    public static AutoHookFunction<WindowProc> wndproc_ = new(new Pattern<int>("55 8B EC 51 8B 45 0C 89 45 FC").Search().Pointer, wndproc);

}