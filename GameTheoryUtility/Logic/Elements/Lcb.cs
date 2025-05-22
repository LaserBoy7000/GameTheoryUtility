namespace GameTheoryUtility.Logic.Elements;

public class Lcb(IElement element) : IElement
{
    public IElement Element { get; set; } = element;
}