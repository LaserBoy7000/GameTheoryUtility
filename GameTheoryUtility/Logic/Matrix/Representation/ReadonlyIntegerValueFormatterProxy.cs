namespace GameTheoryUtility.Logic.Matrix.Representation;

public class ReadonlyIntegerValueFormatterProxy(ReadonlyMatrix<int> matrix, int i, int j) : IValueFormatterProxy
{
    public string Value
    {
        get => matrix[i, j].ToString();
        set => throw new NotSupportedException();
    }

    public bool Validate(string value, out string? error)
    {
        error = null;
        try
        {
            int.Parse(value);
            return true;
        }
        catch
        {
            error = "Неправильне значення";
            return false;
        }
    }
}
