using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Elements;

public class Par(params IElement[] elements) : IElement
{
    [JsonConstructor]
    private Par() : this([]) { }

    public Par(string text) : this(new Tx(text)) { }

    public List<IElement> Elements { get; set; } = elements.ToList();
    public ParagraphType Type { get; set; } = ParagraphType.Plain;
}
