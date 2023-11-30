using Reloaded.Memory.Pointers;
using Silk.NET.Direct3D9;
using SuperCowAPI.Game;
using SuperCowAPI.Memory;
using TerraFX.Interop.DirectX;

namespace SuperCowAPI.SDK;

public static unsafe class DirectX
{
    private static  IntPtr** d3dDevice_ = null;

    public static IntPtr** d3dDevice
    {
        get
        {
            if(d3dDevice_==null) GetDirectDevice();
            return d3dDevice_;
        }
        set
        {
            d3dDevice_ = value;
        }
    }
    public static Silk.NET.Direct3D9.IDirect3DDevice9* D3dDevice => (IDirect3DDevice9*)(*d3dDevice);
    static  List<Ptr<AssetPool.Asset.IDirect3DTexture8>> removedTextures=new();

    public static void GetDirectDevice()
    {
        Pattern<Ptr<Ptr<IntPtr>>> pushD3dDevicePointer=new Pattern<Ptr<Ptr<IntPtr>>>("68 ? ? ? ? 68 ? ? ? ? 6A ? 8B 4D 08").Search(out var result);
        d3dDevice = (IntPtr**)(*result.AddByteOffset(1).Pointer).Pointer;
    }
    public static void Init()
    {

        GetDirectDevice();
        EventManager.On<TickEvents.AfterTickEvent>(delegate () {
            //ReleaseRemovedTextures();
        });
    }

      
    public static void RemoveTexture(AssetPool.Asset.IDirect3DTexture8* texture) {
        if (texture==null) return;
        if (removedTextures.Contains(texture)) return;
        removedTextures.Add(texture);
    }
}