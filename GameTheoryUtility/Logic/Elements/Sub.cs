using GameTheoryUtility.Logic.Visual;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Elements;

public class Sub(IElement main, IElement subscript) : IElement
{
    [JsonConstructor]
    private Sub() : this((IElement?)null!, null!) { }

    public Sub(string main, string subscript) : this(new Tx(main), new Tx(subscript)) { }

    public IElement Main { get; set; } = main;
    public IElement Subscript { get; set; } = subscript;
}
