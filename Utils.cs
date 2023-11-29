using PInvoke;

namespace SuperCowAPI;

public static class Utils
{
    public static void handle_error(Action fn, string where)
    {
        try
        {
            fn();
        }
        catch (Exception e)
        {
            User32.MessageBox(IntPtr.Zero, $"Произошла ошибка {where}:\r\n{e.ToString()}", "SuperMod",
                User32.MessageBoxOptions.MB_OK);
            Environment.Exit(1);
        }
    }
}