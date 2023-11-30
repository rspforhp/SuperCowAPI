using System.Diagnostics;
using PInvoke;
using Reloaded.Memory.Pointers;

namespace SuperCowAPI.Memory;

public unsafe struct Pattern<T> where T : unmanaged
{
    
    public string Sig;
    public Ptr<T> Found;
    public bool FailedToFind;

    public Pattern(string sig)
    {
        
        this.Sig = sig.Replace("?","??");
        Found.Pointer = null;
        FailedToFind = false;
    }

    public static Reloaded.Memory.Sigscan.Scanner Scanner =
        new Reloaded.Memory.Sigscan.Scanner(Process.GetCurrentProcess());
    public Pattern<T> Search(out Ptr<T> result)
    {
        if (Found.Pointer == null)
        {
            var r = Scanner.FindPattern_Simple(Sig);
            Found.Pointer=(T*)(r.Offset+ 0x400000);
            FailedToFind = !r.Found;
            result = Found;
        }
        result= Found;
        return this;
    }
    public Ptr<T> Search()
    {
        if (Found.Pointer == null)
        {
            var r = Scanner.FindPattern_Simple(Sig);
            Found.Pointer=(T*)(r.Offset+ 0x400000);
            FailedToFind = !r.Found;
            return Found;
        }
        return Found;
    }
    public unsafe IntPtr SearchPtr()
    {
        Search(out var test);
        return new IntPtr(test);
    }
}