using System.Numerics;
using Raylib_cs;

namespace ParallelMandelbrot;

public class InputProcessor
{
    private const float WheelMoveRatio = 0.2f;
    private const float ImageMoveRatio = 0.01f;

    private int _lastKey;
    
    public void ProcessKeys()
    {
        var mouseWheelMove = Raylib.GetMouseWheelMove();
    
        Mandelbrot.Height -= mouseWheelMove * Mandelbrot.Height * WheelMoveRatio;
        Mandelbrot.Width -= mouseWheelMove * Mandelbrot.Width * WheelMoveRatio;

        int key = Raylib.GetKeyPressed();
        if (key != 0) _lastKey = key;
        if (Raylib.IsKeyDown((KeyboardKey)_lastKey))
        {
            MatchKeyAction(_lastKey);
        }
    }

    private void MatchKeyAction(int keyInput)
    {
        var key = (KeyboardKey)keyInput;

        switch (key)
        {
            case KeyboardKey.Q:
                DynamicRules.ShowDiagnosticInfo = !DynamicRules.ShowDiagnosticInfo;
                break;
            case KeyboardKey.P:
                DynamicRules.EvalMode = EvalMode.Parallel;
                break;
            case KeyboardKey.S:
                DynamicRules.EvalMode = EvalMode.Sequential;
                break;
            case KeyboardKey.R:
                Mandelbrot.ToInitialState();
                break;
            case KeyboardKey.Equal: // Plus on keyboard
            case KeyboardKey.Minus:
                SetIterationCount(key);
                break;
            case KeyboardKey.Up:
            case KeyboardKey.Down:
            case KeyboardKey.Left:
            case KeyboardKey.Right:
                SetCentre(key);
                break;
        }
    }

    private void SetIterationCount(KeyboardKey key)
    {
        if (key == KeyboardKey.Equal)
            Mandelbrot.Iterations++;
        else Mandelbrot.Iterations--;
    }
    
    private void SetCentre(KeyboardKey key)
    {
        double baseValue = Mandelbrot.Height * ImageMoveRatio;
        (double,double) step = key switch
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
}