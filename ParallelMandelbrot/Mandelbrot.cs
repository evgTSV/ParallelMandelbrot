using System.Drawing;
using System.Numerics;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace ParallelMandelbrot;

public static unsafe class Mandelbrot
{
    private static Color* _pixels = Raylib.New<Color>(0);
    private static Size _textureSize;
    
    public static Complex Center = new(0.0f, -0.75f);
    
    public static float Width = 2.5f;
    public static float Height = 2.5f;

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
    
    private static bool IsMandelbrot(Complex number, int iterations)
    {
        var z = default(Complex);

        int acc = 0;
        while (acc < iterations && z.Magnitude < 2.0f)
        {
            z = z * z + number;
            acc++;
        }
        
        return iterations == acc;
    }

    /// <summary>
    /// Parallel Fork/Join
    /// </summary>
    public static Texture2D CreateTextureParallel()
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
                    
                    if (IsMandelbrot(complex, 100)) _pixels[row * TextureSize.Width + col] = Color.Green;
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
    public static Texture2D CreateTexture()
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
                    
                    if (IsMandelbrot(complex, 100)) _pixels[row * TextureSize.Width + col] = Color.Green;
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