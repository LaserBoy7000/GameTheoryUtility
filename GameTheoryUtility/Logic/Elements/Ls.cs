using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Elements;

public class Ls(params Par[] elements) : IElement
{
    [JsonConstructor]
    private Ls() : this([]) { }

    public List<Par> Items { get; set; } = elements.ToList();
}
