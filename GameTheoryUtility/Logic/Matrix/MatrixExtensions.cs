using System.Numerics;
using GameTheoryUtility.Logic.Elements;
using GameTheoryUtility.Logic.Solvers;
using GameTheoryUtility.Controls;

namespace GameTheoryUtility.Logic.Matrix;

public static class MatrixExtensions
{
    public static LinearProgrammingOptimizationProblem ToOptimizationProblem(this ReadonlyMatrix<double> matrix, int visualizationDecimalPrecision = 2)
    {
        var coefficients = new double[matrix.Size.Columns+matrix.Size.Rows, matrix.Size.Rows];
        
        for (var j = 0; j < matrix.Size.Columns; j++)
            for(var i = 0; i < matrix.Size.Rows; i++)
                coefficients[j, i] = matrix[i, j];
        for (var j = 0; j < matrix.Size.Rows; j++)
            coefficients[j + matrix.Size.Columns, j] = 1.0;
        var equation = Enumerable.Repeat(1.0, matrix.Size.Rows).ToArray();
        var side = Enumerable.Repeat(1.0, matrix.Size.Columns).Concat(Enumerable.Repeat(0.0, matrix.Size.Rows)).ToArray();
        var signs = Enumerable.Repeat(InequalityType.GreaterOrEqual, matrix.Size.Columns + matrix.Size.Rows).ToArray();
        return new(coefficients, signs, side, equation, OptimizationType.Minimize, visualizationDecimalPrecision);
    }
    public static Matrix<double> ToReal(this IMutableMatrix matrix)
    {
        if (matrix is Matrix<double> m)
            return m;
        if (matrix is not Matrix<int> integer)
            return null!;
        var results = new Matrix<double>(matrix.Size.Rows, matrix.Size.Columns);
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                results[i, j] = double.CreateChecked(integer[i, j]);
        return results;
    }

    public static ReadonlyMatrix<double> ToReal(this IMatrix matrix)
    {
        if (matrix is ReadonlyMatrix<double> m)
            return m;
        if (matrix is not ReadonlyMatrix<int> integer)
            return null!;
        var values = new double[matrix.Size.Rows, matrix.Size.Columns];
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                values[i, j] = double.CreateChecked(integer[i, j]);
        return new ReadonlyMatrix<double>(matrix.Size.Rows, matrix.Size.Columns, values);
    }

    public static Matrix<int> Round(this IMutableMatrix matrix)
    {
        if (matrix is Matrix<int> m)
            return m;
        if (matrix is not Matrix<double> real)
            return null!;
        var results = new Matrix<int>(matrix.Size.Rows, matrix.Size.Columns);
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                results[i, j] = (int)(double.CreateChecked(real[i, j]) + .5);
        return results;
    }

    public static ReadonlyMatrix<int> Round(this IMatrix matrix)
    {
        if (matrix is ReadonlyMatrix<int> m)
            return m;
        if (matrix is not ReadonlyMatrix<double> real)
            return null!;
        var values = new int[matrix.Size.Rows, matrix.Size.Columns];
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                values[i, j] = (int)(double.CreateChecked(real[i, j]) + .5);
        return new ReadonlyMatrix<int>(matrix.Size.Rows, matrix.Size.Columns, values);
    }

    public static IMutableMatrix Clone(this IMutableMatrix matrix)
    {
        if(matrix is Matrix<int> integer)
        {
            var res = new Matrix<int>(matrix.Size.Rows, matrix.Size.Columns);
            integer.Iterate((m, i, j) => res[i, j] = m[i, j]);
            return res;
        }
        if(matrix is Matrix<double> real)
        {
            var res = new Matrix<double>(matrix.Size.Rows, matrix.Size.Columns);
            real.Iterate((m, i, j) => res[i, j] = m[i, j]);
            return res;
        }
        return null!;
    }

    public static void MigrateValues(this IMutableMatrix matrix, IMutableMatrix source)
    {
        if (matrix is Matrix<int> i)
            i.Round().MigrateValues(source.Round());
        else
            matrix.ToReal().MigrateValues(source.ToReal());
    }

    public static void MigrateValues<T>(this Matrix<T> target, Matrix<T> source) where T : INumber<T>
    {
        var rows = int.Min(target.Size.Rows, source.Size.Rows);
        var cols = int.Min(target.Size.Columns, source.Size.Columns);
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                target[i, j] = source[i, j];
    }

    public static MatrixViewModel CreateViewModel(this IMutableMatrix matrix, IMatrixGameParameters parameters) => matrix switch
    {
        Matrix<int> i => new MatrixViewModel(i),
        _ => new MatrixViewModel(matrix.ToReal(), parameters.DecimalPrecision),
    };

    public static void Iterate<T>(this Matrix<T> matrix, MatrixIteratorAction<Matrix<T>> action) where T : INumber<T>
    {
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                action(matrix, i, j);
    }

    public delegate void MatrixIteratorAction<TMatrix>(TMatrix matrix, int i, int j) where TMatrix : IMatrix;

    public static void Iterate<T>(this ReadonlyMatrix<T> matrix, MatrixIteratorAction<ReadonlyMatrix<T>> action) where T : INumber<T>
    {
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                action(matrix, i, j);
    }

    public static Mat ToElement(this ReadonlyMatrix<double> matrix, int decimalPrecision = 2)
    {
        List<string> rendered = [];
        matrix.Iterate((m, i, j) => rendered.Add(m[i, j].ToString($"F{decimalPrecision}")));
        return new Mat(matrix.Size.Rows, matrix.Size.Columns, rendered.Select(x => new Tx(x)).ToArray());
    }

    public static Mat ToElement(this ReadonlyMatrix<int> matrix)
    {
        List<string> rendered = [];
        matrix.Iterate((m, i, j) => rendered.Add(m[i, j].ToString()));
        return new Mat(matrix.Size.Rows, matrix.Size.Columns, rendered.Select(x => new Tx(x)).ToArray());
    }

    public static Mat ToElement(this IMatrix matrix, int decimalPrecision = 2) => matrix switch
    {
        ReadonlyMatrix<double> real => real.ToElement(decimalPrecision),
        ReadonlyMatrix<int> integer => integer.ToElement(),
        _ => null!
    };
}
