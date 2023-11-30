
using Microsoft.Collections.Extensions;
using Reloaded.Memory.Pointers;

namespace SuperCowAPI;

public static class EventManager
{
    public static Dictionary<Type, List<Delegate>> Events=new Dictionary<Type, List<Delegate>>();

    public static void On<T>(T del) where T : Delegate
    {
        if (!Events.ContainsKey(typeof(T))) Events[typeof(T)] = new List<Delegate>();
        Events[typeof(T)].Add(del);
    }

    public static object Emit<T>(params object[] parameters) where T : Delegate
    {
        var b= Events.TryGetValue(typeof(T), out var delegs);
        if (!b) return null;
        object result = null;
        foreach (var del in delegs)
        {
            result=del.DynamicInvoke(parameters);
        }
        return result;
    }
}

