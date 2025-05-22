using System.Globalization;

namespace GameTheoryUtility.Logic.Matrix.Representation;

public class DoubleValueFormatterProxy(Matrix<double> matrix, int i, int j, int decimalPrecision) : IValueFormatterProxy
{
    static readonly CultureInfo CULTURE = new("uk") { NumberFormat = { NumberDecimalSeparator = "," } };

    public string Value
    {
        get => matrix[i, j].ToString($"F{decimalPrecision}");
        set => matrix[i, j] = Math.Round(double.Parse(value.Replace('.', ','), CULTURE), decimalPrecision);
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
