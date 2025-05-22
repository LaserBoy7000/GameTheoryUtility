namespace GameTheoryUtility.Logic.Elements;

public class Ovr(IElement operation, IElement operand, IElement subscript) : IElement
{
    public IElement Operation { get; set; } = operation;
    public IElement Operand { get; set; } = operand;
    public IElement Subscript { get; set; } = subscript;
}