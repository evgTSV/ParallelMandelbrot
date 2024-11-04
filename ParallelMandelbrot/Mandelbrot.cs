using System.Drawing;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace ParallelMandelbrot;

public enum EvalMode
{
    Sequential,
    Parallel
}

public static unsafe class Mandelbrot
{
    static Mandelbrot()
    {
        ToInitialState();
    }
    
    public static void ToInitialState()
    {
        Center = new(0.0f, -0.75f);
        Width = 2.5f;
        Height = 2.5f;
        Iterations = 100;
    }
    
    private static Color* _pixels = Raylib.New<Color>(0);
    private static Size _textureSize;
    
    public static Complex Center;
    
    public static float Width;
    public static float Height;

    public static int Iterations;

    public static Size TextureSize
    {
        get => _textureSize;
        set
        {
            Raylib.MemFree(_pixels);
            _pixels = Raylib.New<Color>(value.Height * value.Width);
            _textureSize = value;
        }
    }

    private static double ComputeRow(int col, int countCols)
        => Center.Real - Width / 2.0f + col * Width / countCols;

    private static double ComputeColumn(int row, int countRows)
        => Center.Imaginary - Height / 2.0f + row * Width / countRows;

    public static string Hash()
    {
        using var sha256 = SHA256.Create();

        var sb = new StringBuilder()
            .Append(Center)
            .Append(Width)
            .Append(Height)
            .Append(Iterations);

        var data = Encoding.UTF8.GetBytes(sb.ToString());
        var hash = sha256.ComputeHash(data);

        return BitConverter.ToString(hash);
    }
    
    private static bool IsMandelbrot(Complex number)
    {
        var z = default(Complex);

        int acc = 0;
        while (acc < Iterations && z.Magnitude < 2.0f)
        {
            z = z * z + number;
            acc++;
        }
        
        return Iterations == acc;
    }

    public static Texture2D CreateTexture(EvalMode mode) =>
        mode switch
        {
            EvalMode.Parallel => CreateTextureParallel(),
            EvalMode.Sequential => CreateTextureSequential(),
            _ => throw new ArgumentException("Mode not supported")
        };

    /// <summary>
    /// Parallel Fork/Join
    /// </summary>
    private static Texture2D CreateTextureParallel()
    {
        try
        {
            Parallel.For(0, TextureSize.Height - 1, row =>
            {
                for (int col = 0; col < TextureSize.Width; col++)
                {
                    double x = ComputeRow(row, TextureSize.Height);
                    double y = ComputeColumn(col, TextureSize.Width);

                    var complex = new Complex(y, x);
                    
                    if (IsMandelbrot(complex)) _pixels[row * TextureSize.Width + col] = Color.Green;
                    else _pixels[row * TextureSize.Width + col] = Color.Black;
                }
            });
            
            return CreateTextureFromPixels(_pixels, TextureSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Raylib.MemFree(_pixels);
            throw;
        }
    }
    
    /// <summary>
    /// One-core sequential processing
    /// </summary>
    private static Texture2D CreateTextureSequential()
    {
        try
        {
            for (int row = 0; row < TextureSize.Height; row++)
            {
                for (int col = 0; col < TextureSize.Width; col++)
                {
                    double x = ComputeRow(row, TextureSize.Height);
                    double y = ComputeColumn(col, TextureSize.Width);

                    var complex = new Complex(y, x);
                    
                    if (IsMandelbrot(complex)) _pixels[row * TextureSize.Width + col] = Color.Green;
                    else _pixels[row * TextureSize.Width + col] = Color.Black;
                }
            }
            
            return CreateTextureFromPixels(_pixels, TextureSize);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Raylib.MemFree(_pixels);
            throw;
        }
    }

    private static Texture2D CreateTextureFromPixels(Color* pixels, Size size)
    {
        var mandelbrotImg = new Image
        {
            Data = pixels,
            Format = PixelFormat.UncompressedR8G8B8A8,
            Height = size.Height,
            Width = size.Width,
            Mipmaps = 1
        };
        
        var texture = Raylib.LoadTextureFromImage(mandelbrotImg);

        return texture;
    }
}