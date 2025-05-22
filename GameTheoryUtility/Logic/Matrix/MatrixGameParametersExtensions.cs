using DocumentFormat.OpenXml.Spreadsheet;
using System.Reflection.Emit;

namespace GameTheoryUtility.Logic.Matrix;

public static class MatrixGameParametersExtensions
{
    public static IMutableMatrix CreateMutableGameMatrix(this IMatrixGameParameters parameters) => parameters.IsInteger ?
        new Matrix<int>(parameters.StrategiesCountA, parameters.StrategiesCountB) :
        new Matrix<double>(parameters.StrategiesCountA, parameters.StrategiesCountB);

    public static string FormatI(this IMatrixGameParameters parameters, double value) => parameters.IsInteger ? ((int)value).ToString() : value.ToString($"F{parameters.DecimalPrecision}");

    public static string FormatC(this IMatrixGameParameters parameters, double value) => value.ToString($"F{parameters.DecimalPrecision}");

    public static IMutableMatrix GenerateMutableGameMatrix(this GeneratedMatrixGameParameters parameters)
    {
        var matrix = parameters.CreateMutableGameMatrix();
        if(parameters.GeneratorSeed == null)
            parameters.GeneratorSeed = DateTime.Now.GetHashCode();
       
        if(matrix is Matrix<int> integer)
        {
            if (parameters.NoRepeats)
                new UniqueMedianIntegerFiller(parameters, integer).Fill();
            else
                new IntegerMatrixFiller(parameters, integer).Fill();
        }

        return matrix;
    }
}