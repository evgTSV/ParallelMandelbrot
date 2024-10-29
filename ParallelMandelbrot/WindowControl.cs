using System.Drawing;
using Raylib_cs;
using Color = Raylib_cs.Color;
using Rectangle = Raylib_cs.Rectangle;

namespace ParallelMandelbrot;

public static class WindowControl
{
    private static object _locker = new();
    
    public static void OpenWindow(Size windowSize, string title)
    {
        lock (_locker)
        {
            if (Raylib.IsWindowReady()) throw new InitializationWindowException("Window already opened");
        
            Raylib.InitWindow(windowSize.Width, windowSize.Height, title);
            Raylib.SetTargetFPS(60);
        }
    }
    
    public static void CloseWindow()
    {
        lock (_locker)
        {
            if (Raylib.IsWindowReady())
                Raylib.CloseWindow();
        }
    }
}