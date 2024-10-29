using System.Drawing;
using System.Numerics;
using ParallelMandelbrot;
using Raylib_cs;
using Color = Raylib_cs.Color;

var size = new Size(500, 500);

WindowControl.OpenWindow(size, "Mandelbrot");
Mandelbrot.TextureSize = size;

var texture = default(Texture2D);
int lastKey = 0;

while (!Raylib.WindowShouldClose())
{
    var mouseWheelMove = Raylib.GetMouseWheelMove();
    
    Mandelbrot.Height -= mouseWheelMove * Mandelbrot.Height * 0.2f;
    Mandelbrot.Width -= mouseWheelMove * Mandelbrot.Width * 0.2f;

    int key = Raylib.GetKeyPressed();
    if (key != 0) lastKey = key;
    if (Raylib.IsKeyDown((KeyboardKey)lastKey))
        SetCentre(lastKey);
    
    texture = Mandelbrot.CreateTextureParallel();
    
    Raylib.BeginDrawing();
    
    Raylib.DrawTexture(texture, 0, 0, Color.RayWhite);
    Raylib.DrawFPS(10, 10);
    
    Raylib.EndDrawing();
    Raylib.UnloadTexture(texture);
}
Raylib.UnloadTexture(texture);
WindowControl.CloseWindow();

static void SetCentre(int keyInput)
{
    double baseValue = Mandelbrot.Height * 0.01;
    (double,double) step = (KeyboardKey)keyInput switch
    {
        KeyboardKey.Up => (baseValue, 0),
        KeyboardKey.Down => (-baseValue, 0),
        KeyboardKey.Left => (0, baseValue),
        KeyboardKey.Right => (0, -baseValue),
        _ => (0, 0)
    };
    
    Mandelbrot.Center = new Complex(
        Mandelbrot.Center.Real - step.Item1,
        Mandelbrot.Center.Imaginary - step.Item2);
}