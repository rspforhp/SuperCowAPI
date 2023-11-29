using Reloaded.Memory.Pointers;
using SuperCowAPI.Memory;
using TerraFX.Interop.DirectX;

namespace SuperCowAPI.SDK;

public static unsafe class DirectX
{
    public static  ID3D10Device** d3dDevice = null;
    public static  ID3D10Device* D3dDevice =>*d3dDevice;
    public static void Init() {
        Pattern<Ptr<Ptr<ID3D10Device>>> pushD3dDevicePointer=new Pattern<Ptr<Ptr<ID3D10Device>>>("68 ? ? ? ? 68 ? ? ? ? 6A ? 8B 4D 08").Search(out var result);
        d3dDevice = (ID3D10Device**)(*result.AddByteOffset(1).Pointer).Pointer;

        //TODO: EventManager
        //EventManager::On<AfterTickEvent>([] {
        //    ReleaseRemovedTextures();
        //});
    }
}