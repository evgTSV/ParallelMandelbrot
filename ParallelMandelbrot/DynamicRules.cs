namespace ParallelMandelbrot;

public static class DynamicRules
{
    public static EvalMode EvalMode { get; set; } = EvalMode.Parallel;
    public static bool ShowDiagnosticInfo { get; set; } = true;
}