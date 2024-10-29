namespace ParallelMandelbrot;

[Serializable]
public class InitializationWindowException(string message) : Exception
{
    public override string Message => message;
}