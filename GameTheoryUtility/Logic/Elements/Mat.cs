using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Elements;

public class Mat(int rows, int columns, params IElement[] elements) : IElement
{
    [JsonConstructor]
    private Mat() : this(0, 0) { }

    public int Rows { get; set; } = rows;
    public int Columns { get; set; } = columns;
    public List<IElement> Elements { get; set; } = elements.ToList();
}
