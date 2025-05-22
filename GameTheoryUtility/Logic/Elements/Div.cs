using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Elements;

public class Div(IElement top, IElement bottom) : IElement
{
    public IElement Top { get; set; } = top;
    public IElement Bottom { get; set; } = bottom;
}