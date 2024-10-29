using System.Drawing;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using ParallelMandelbrot;

namespace MandelbrotBenchmark;

[MemoryDiagnoser]
public class MandelbrotTest
{
    public static Complex Center = new Complex(0.0f, -0.75f);
    
    public static float Width = 2.5f;
    public static float Height = 2.5f;

    public static Size TextureSize { get; set; } = default(Size);
    
    private static double ComputeRow(int col, int countCols)
        => Center.Real - Width / 2.0f + (float)col * Width / (float)countCols;

    private static double ComputeColumn(int row, int countRows)
        => Center.Imaginary - Height / 2.0f + (float)row * Width / (float)countRows;
    
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
    
    [GlobalSetup]
    public void Setup() => TextureSize = new Size(2000, 2000);
    
    [Benchmark]
    public void SequentialProcessing()
    {
        var pixels = new Color[TextureSize.Height * TextureSize.Width];
        for (int row = 0; row < TextureSize.Height; row++)
        {
            for (int col = 0; col < TextureSize.Width; col++)
            {
                double x = ComputeRow(row, TextureSize.Height);
                double y = ComputeColumn(col, TextureSize.Width);

                var complex = new Complex(y, x);
                    
                if (IsMandelbrot(complex, 100)) pixels[row * TextureSize.Width + col] = Color.White;
                else pixels[row * TextureSize.Width + col] = Color.Black;
            }
        }
    }

    [Benchmark]
    public void ParallelProcessing()
    {
        var pixels = new Color[TextureSize.Height * TextureSize.Width];
        Parallel.For(0, TextureSize.Height - 1, row =>
        {
            for (int col = 0; col < TextureSize.Width; col++)
            {
                double x = ComputeRow(row, TextureSize.Height);
                double y = ComputeColumn(col, TextureSize.Width);

                var complex = new Complex(y, x);
                    
                if (IsMandelbrot(complex, 100)) pixels[row * TextureSize.Width + col] = Color.White;
                else pixels[row * TextureSize.Width + col] = Color.Black;
            }
        });
    }
    
    [Benchmark]
    public void ParallelProcessingSaturated()
    {
        var pixels = new Color[TextureSize.Height * TextureSize.Width];
        Parallel.For(0, TextureSize.Height - 1, row =>
        {
            Parallel.For(0, TextureSize.Width - 1, col =>
            {
                double x = ComputeRow(row, TextureSize.Height);
                double y = ComputeColumn(col, TextureSize.Width);

                var complex = new Complex(y, x);

                if (IsMandelbrot(complex, 100)) pixels[row * TextureSize.Width + col] = Color.White;
                else pixels[row * TextureSize.Width + col] = Color.Black;
            });
        });
    }
}