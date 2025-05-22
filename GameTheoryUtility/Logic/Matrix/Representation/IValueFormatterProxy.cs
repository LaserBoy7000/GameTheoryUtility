namespace GameTheoryUtility.Logic.Matrix.Representation;

public interface IValueFormatterProxy
{
    public string Value { get; set; }
    public bool Validate(string value, out string? error);
}
