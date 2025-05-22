using System.Globalization;

namespace GameTheoryUtility.Logic.Matrix.Representation;

public class ReadonlyDoubleValueFormatterProxy(ReadonlyMatrix<double> matrix, int i, int j, int decimalPrecision) : IValueFormatterProxy
{
    static readonly CultureInfo CULTURE = new("uk") { NumberFormat = { NumberDecimalSeparator = "," } };

    public string Value
    {
        get => matrix[i, j].ToString($"F{decimalPrecision}");
        set => throw new NotSupportedException();
    }

    public bool Validate(string value, out string? error)
    {
        error = null;
        try
        {
            double.Parse(value.Replace('.', ','), CULTURE);
            return true;
        }
        catch
        {
            error = "Неправильне значення";
            return false;
        }
    }
}
