using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace SuperCowAPI;

public class Config : IDisposable
{
    static string _cfgPath ="";
    static YamlMappingNode _cfg = new YamlMappingNode();

    public static YamlMappingNode Get()
    {
        return _cfg;
    }
    public static void Set<T>(string key,T value)
    {
        Config.Get().Children[key] = Extension.MakeFrom(value);
    }
    public static YamlNode GetAndEnsureKey(string key)
    {
        var k = _cfg.Children;
        if (!k.ContainsKey(key))
        {
            k[key] = null;
        }
        return k[key];
    }
    public YamlMappingNode data;

    public Config()
    {
        data = new YamlMappingNode(_cfg as YamlNode);
    }

    private void ReleaseUnmanagedResources()
    {
        Save();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~Config()
    {
        ReleaseUnmanagedResources();
    }

    public static void Init()
    {
        _cfgPath =Directory.GetCurrentDirectory()+"\\modcfg.yml";
        if (File.Exists(_cfgPath)) _cfg =deserializer.Deserialize<YamlMappingNode>(File.ReadAllText(_cfgPath));
    }

    private static Serializer serializer = new Serializer();
    private static Deserializer deserializer = new Deserializer();
    public static void Save()
    {
        var s = serializer.Serialize(_cfg);
        File.WriteAllText(_cfgPath,
            s);
    }


}