using Reloaded.Memory.Pointers;
using YamlDotNet.RepresentationModel;

namespace SuperCowAPI;

public static unsafe class Extension
{
    public static Ptr<T> AddByteOffset<T>(this Ptr<T> t,int offset) where T : unmanaged
    {
        return new Ptr<T>((T*)(((int)t.Pointer) + offset));
    }
    public static string? GetString(this YamlNode a)
    {
        if (a == null|| a.ToString()=="") return null;
        return a.ToString();
    }
    public static bool? GetBool(this YamlNode a)
    {
        if (a == null || a.ToString()=="") return null;
        return bool.Parse(a.ToString());
    }
    public static double? GetDouble(this YamlNode a)
    {
        if (a == null|| a.ToString()=="") return null;
        return Double.Parse(a.ToString());
    }

    public static YamlNode MakeFrom<T>(T a) 
    {
        return a.ToString();
    }
}