using DocumentFormat.OpenXml.Packaging;

namespace GameTheoryUtility.Logic.External.Word;

public interface IWordDocumentStyle
{
    public string Name { get; }
    public void Add(MainDocumentPart main);

    public static string HEADER = "Header";
    public static string NORMAL = "Normal";
}
