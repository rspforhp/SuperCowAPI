using System.Numerics;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Pointers;
using SuperCowAPI.Memory;
using TerraFX.Interop.DirectX;

namespace SuperCowAPI.Game;

public unsafe struct AssetPool
{
    private static AssetPool* _instance;
    public static AssetPool* Instance()
    {
        if (_instance != null) return _instance;
        Pattern<Ptr<AssetPool>> pat=new Pattern<Ptr<AssetPool>>("B9 ? ? ? ? E8 ? ? ? ? 68 ? ? ? ? A1 ? ? ? ? 8B 08").Search(out var  res); //  mov ecx, offset AssetPool__instance
        _instance = *res.AddByteOffset(1).Pointer;
        return _instance;
    }
    public struct AssetMeta
    {
        bool notFound = false;
        private CharPointerString origDir = "";
        private CharPointerString origName = "";
        Vector2 canvasSizeMultiplier =new Vector2(1,1);
        bool loadedManually = false;

        public AssetMeta()
        {
        }

    };
    public struct Asset {
        fixed char name[124]; // originally name has length of 128, i take 4 bytes for custom meta ptr
        AssetMeta* meta;
        uint width;
        uint height;

        public struct IDirect3DTexture8
        {
            
        }
        IDirect3DTexture8* texture;
        byte hasAlpha;
        byte isPoolDefault;
        int format;

        void ReplaceTexture(IDirect3DTexture8* tex)
        { 
            SDK.DirectX.RemoveTexture(texture);
            texture = tex;
        }

    };
    public void FreeAsset(Asset* asset) {
        Pattern<int> pat=new Pattern<int>("E8 ? ? ? ? C7 05 ? ? ? ? ? ? ? ? 68 ? ? ? ? FF 15").Search(out var  mem);
        mem=mem.GoToNearCall();
        FunctionPtr<FreeAsset_> a = new FunctionPtr<FreeAsset_>((ulong)mem.Pointer);
        fixed (AssetPool* t = &this)
        {
            a.GetDelegate()(t, asset);
        }
    }

    [Function(CallingConventions.MicrosoftThiscall)]
    public delegate void FreeAsset_(void* instance, void* asset);
    static  List<Ptr<Asset>> removedAssets=new List<Ptr<Asset>>();
    public void FreeRemovedAssets() {
        if (removedAssets.Count==0) return;
         var scheduledAssets = removedAssets;
        removedAssets =new List<Ptr<Asset>>() {};
        foreach (var asset in scheduledAssets) {
            FreeAsset(asset);
        }
    }

    
    public static void Init() {
        var instance = Instance();
        EventManager.On<TickEvents.AfterTickEvent>(delegate  {
            instance->FreeRemovedAssets();
        });
    }
}