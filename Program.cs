using OpenTK.Windowing.Desktop;

public static class Program
{
    private static void Main()
    {
        var nativeWindowsettings = new NativeWindowSettings()
        {
            ClientSize = (800, 600),
            Title = "Hi watch",
        };

        using (var window = new Window(GameWindowSettings.Default, nativeWindowsettings))
        {
            window.Run();
        }
    }
}