using GameTheoryUtility.Logic.Elements;

namespace GameTheoryUtility.Logic.Computations;

public class DescriptionComputationStep(Par paragraph, string? tag = null) : ComputationStep(tag)
{
    public Par Paragraph { get; set; } = paragraph;

    public override void Render(IVisualizationEngine engine)
    {
        engine.RenderElement(Paragraph);
    }
}
