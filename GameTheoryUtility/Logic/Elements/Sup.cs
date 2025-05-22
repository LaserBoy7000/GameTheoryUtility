namespace GameTheoryUtility.Logic.Elements;

public class Sup(IElement main, IElement superscript) : IElement
{
    public IElement Main { get; set; } = main;
    public IElement Superscript { get; set; } = superscript;
}
