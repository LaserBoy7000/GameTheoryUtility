using GameTheoryUtility.Logic.Matrix.Representation;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Matrix;

[JsonConverter(typeof(MatrixJsonConverter))]
public class ReadonlyMatrix<T>(int rows, int columns, T[,] values) : IMatrix
{
    protected readonly T[,] _values = values;

    public MatrixSize Size { get; } = new(rows, columns);

    public T this[int i, int j] => _values[i, j];
}
