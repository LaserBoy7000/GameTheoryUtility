namespace GameTheoryUtility.Logic.Elements;

public class Tx(string value) : IElement
{
    public string Value { get; set; } = value;
}
