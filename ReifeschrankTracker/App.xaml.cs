using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace ReifeschrankTracker;

public partial class App : Application
{
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

    private const int DwmwaUseImmersiveDarkMode = 20;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // Apply dark title bar to every Window that loads in the application.
        EventManager.RegisterClassHandler(
            typeof(Window),
            Window.LoadedEvent,
            new RoutedEventHandler(ApplyDarkTitleBar));
    }

    private static void ApplyDarkTitleBar(object sender, RoutedEventArgs e)
    {
        if (sender is not Window w) return;
        var hwnd = new WindowInteropHelper(w).Handle;
        if (hwnd == IntPtr.Zero) return;
        int value = 1;
        DwmSetWindowAttribute(hwnd, DwmwaUseImmersiveDarkMode, ref value, sizeof(int));
    }
}
