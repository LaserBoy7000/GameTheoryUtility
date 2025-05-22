using GameTheoryUtility.Logic.Matrix.Representation;
using System.Numerics;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Matrix;

[JsonConverter(typeof(MutableMatrixJsonConverter))]
public class Matrix<T>(int rows, int columns, T[,] values) : ReadonlyMatrix<T>(rows, columns, values), IMutableMatrix where T : INumber<T>
{
    public Matrix(int rows, int columns) : this(rows, columns, new T[rows, columns]) { }

    public new T this[int i, int j]
    {
        get => _values[i, j];
        set => _values[i, j] = value;
    }

    //public Matrix<double> ToReal()
    //{
    //    if (this is Matrix<double> m)
    //        return m;
    //    var results = new Matrix<double>(Size.Rows, Size.Columns);
    //    for (int i = 0; i < Size.Rows; i++)
    //        for (int j = 0; j < Size.Columns; j++)
    //            results[i, j] = double.CreateChecked(_values[i, j]);
    //    return results;
    //}

    //public Matrix<int> Round()
    //{
    //    if (this is Matrix<int> m)
    //        return m;
    //    var results = new Matrix<int>(Size.Rows, Size.Columns);
    //    for (int i = 0; i < Size.Rows; i++)
    //        for (int j = 0; j < Size.Columns; j++)
    //            results[i, j] = (int)(double.CreateChecked(_values[i, j]) + .5);
    //    return results;
    //}
    public IMatrix MakeReadonly() => AsReadonly;
    public ReadonlyMatrix<T> AsReadonly => new(Size.Rows, Size.Columns, (T[,])_values.Clone());
}
