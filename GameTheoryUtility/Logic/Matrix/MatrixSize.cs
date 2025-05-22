namespace GameTheoryUtility.Logic.Matrix;

public readonly struct MatrixSize(int rows, int columns)
{
    public int Rows { get; } = rows;
    public int Columns { get; } = columns;
}