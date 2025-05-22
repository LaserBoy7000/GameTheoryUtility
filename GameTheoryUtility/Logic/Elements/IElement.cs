using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Elements;

[JsonDerivedType(typeof(Div), "fraction")]
[JsonDerivedType(typeof(Tx), "text")]
[JsonDerivedType(typeof(Mat), "matrix")]
[JsonDerivedType(typeof(Sub), "subscript")]
[JsonDerivedType(typeof(Sup), "superscript")]
[JsonDerivedType(typeof(Par), "paragraph")]
[JsonDerivedType(typeof(Ls), "list")]
[JsonDerivedType(typeof(Ovr), "over")]
[JsonDerivedType(typeof(Lcb), "lbracket")]
public interface IElement { }
