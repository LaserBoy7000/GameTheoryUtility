using GameTheoryUtility.Logic.Visual;

namespace GameTheoryUtility.Logic.Computations;

public class GraphComputationStep(Graph graph, string? tag = null) : ComputationStep(tag)
{
    public Graph Graph { get; set; } = graph;

    public override void Render(IVisualizationEngine engine) =>
         engine.DrawVisual((c, s) => c.DrawGraph(s, Graph));
}
