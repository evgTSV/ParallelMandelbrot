using System.Drawing;
using ParallelMandelbrot;
using Raylib_cs;
using Color = Raylib_cs.Color;

var size = new Size(500, 500);

WindowControl.OpenWindow(size, "Mandelbrot");
Mandelbrot.TextureSize = size;

var inputProcessor = new InputProcessor();
var texture = default(Texture2D);
var changed = MandelbrotChanged(String.Empty);

while (!Raylib.WindowShouldClose())
{
    inputProcessor.ProcessKeys();
    
    if (changed())
    {
        Raylib.UnloadTexture(texture);
        texture = Mandelbrot.CreateTexture(DynamicRules.EvalMode);
        changed = MandelbrotChanged(Mandelbrot.Hash());
    }
    
    Raylib.BeginDrawing();
    
    Raylib.DrawTexture(texture, 0, 0, Color.RayWhite);
    Raylib.DrawFPS(10, 10);
    DrawConfigurationInfo();
    
    Raylib.EndDrawing();
}
Raylib.UnloadTexture(texture);
WindowControl.CloseWindow();

static void DrawConfigurationInfo()
{
    Raylib.DrawText($"Iterations: {Mandelbrot.Iterations}", 100, 15, 10, Color.White);
    Raylib.DrawText($"Eval mode: {DynamicRules.EvalMode.ToString()}", 100, 30, 10, Color.White);
}

static Func<bool> MandelbrotChanged(string targetHash) => 
    () => !Mandelbrot.Hash().Equals(targetHash);