namespace GameTheoryUtility.Logic.Computations;

public abstract class ComputationStep(string? tag = null) : IComputationStep
{
    public string? Tag => tag;
    public abstract void Render(IVisualizationEngine engine);
}