using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Computations;

[JsonDerivedType(typeof(DescriptionComputationStep), "description")]
[JsonDerivedType(typeof(GraphComputationStep), "graph")]
public interface IComputationStep
{
    public string? Tag { get; }
    public void Render(IVisualizationEngine engine);
}
