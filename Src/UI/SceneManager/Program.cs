using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System;

namespace SceneManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameSettings = GameWindowSettings.Default;
            var nativeSettings = NativeWindowSettings.Default;

            nativeSettings.APIVersion = new Version(4, 5);
            nativeSettings.Flags = OpenTK.Windowing.Common.ContextFlags.ForwardCompatible;
            nativeSettings.Size = new Vector2i(1920, 1080);

            using var window = new MainWindow(gameSettings, nativeSettings);
            window.Run();
        }
    }
}
