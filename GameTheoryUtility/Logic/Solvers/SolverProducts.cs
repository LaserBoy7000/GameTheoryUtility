namespace GameTheoryUtility.Logic.Solvers;

public class SolverProducts
{
    public int? OptimalCleanStrategyA { get; set; }
    public int? OptimalCleanStrategyB { get; set; }
    public double[]? OptimalMixedStrategyA { get; set; }
    public double[]? OptimalMixedStrategyB { get; set; }
    public double? SaddlePoint { get; set; }
    public double? GameCost { get; set; }
    public (int, int)[]? DominatedA { get; set; }
    public (int, int)[]? DominatedB { get; set; }
}