using GameTheoryUtility.Logic.Elements;
using GameTheoryUtility.Logic.Matrix;
using GameTheoryUtility.Logic.Solvers;
using GameTheoryUtility.Logic.Visual;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Computations;

public class GameComputationInstance
{
    readonly IEnumerable<ISolver>? _solvers;

    public GameComputationInstance(IMatrixGameParameters parameters, IMatrix matrix, IEnumerable<ISolver> solvers)
    {
        Matrix = matrix;
        Parameters = parameters;
        Results = new();
        _solvers = solvers;
    }

    [JsonConstructor]
    public GameComputationInstance(List<IComputationStep> computationSteps, SolverProducts results)
    {
        ComputationSteps = computationSteps;
        Results = results;
    }
    public List<IComputationStep> ComputationSteps { get; } = [];
    public SolverProducts Results { get; }
    [JsonIgnore]
    public IMatrix? Matrix { get; set; }
    public IMatrixGameParameters? Parameters { get; }

    public void Compute()
    {
        if (_solvers == null)
            return;
        foreach (var solver in _solvers)
            solver.Solve(this);
    }

    public void Render(IVisualizationEngine visualization)
    {
        foreach (var step in ComputationSteps)
            step.Render(visualization);
    }
}

public static class GameComputationInstanceExtensions
{
    public static void AddMethodHeader(this GameComputationInstance instance, string header, string? tag = null) =>
        instance.ComputationSteps.Add(new DescriptionComputationStep(new(header) { Type = ParagraphType.Header1 }, tag));

    public static void AddStepHeader(this GameComputationInstance instance, string header, string? tag = null) =>
        instance.ComputationSteps.Add(new DescriptionComputationStep(new(header) { Type = ParagraphType.Header2 }, tag));

    public static void AddParagraph(this GameComputationInstance instance, Par paragraph, string? tag = null) =>
        instance.ComputationSteps.Add(new DescriptionComputationStep(paragraph, tag));

    public static void AddParagraph(this GameComputationInstance instance, string text, string? tag = null) =>
        instance.ComputationSteps.Add(new DescriptionComputationStep(new(new Tx(text)), tag));

    public static void AddGraph(this GameComputationInstance instance, Graph graph, string? tag = null) =>
        instance.ComputationSteps.Add(new GraphComputationStep(graph, tag));
}