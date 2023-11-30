using System.Runtime.InteropServices;

namespace SuperCowAPI;

[StructLayout(LayoutKind.Explicit, Size = 0x4, CharSet = CharSet.Ansi)]
public struct CharPointerString
{
    [FieldOffset(0x0)] public unsafe byte* p;

    public string ManagedString
    {
        get => this;
        set => this = value;
    }

    public override string ToString()
    {
        return this;
    }

    public static implicit operator CharPointerString(string ba)
    {
        var str = Marshal.StringToHGlobalAnsi(ba);
        var b = new CharPointerString();
        unsafe
        {
            b.p = (byte*)str.ToPointer();
        }

        return b;
    }

    public static implicit operator string(CharPointerString ba)
    {
        unsafe
        {
            return Marshal.PtrToStringAnsi(new IntPtr(ba.p));
        }
    }
}