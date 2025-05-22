using GameTheoryUtility.Logic.Matrix;

namespace GameTheoryUtility.Controls;

public class ReadonlyMatrixViewModel
{
    public ReadonlyMatrixViewModel(ReadonlyMatrix<int> matrix)
    {
        Size = matrix.Size;
        List<string> cells = [];
        matrix.Iterate((m, i, j) => cells.Add(m[i, j].ToString()));
        Cells = cells;
    }

    public ReadonlyMatrixViewModel(ReadonlyMatrix<double> matrix, int decimalPrecision)
    {
        Size = matrix.Size;
        List<string> cells = [];
        matrix.Iterate((m, i, j) => cells.Add(m[i, j].ToString($"F{decimalPrecision}")));
        Cells = cells;
    }

    public IEnumerable<string> Cells { get; }

    public MatrixSize Size { get; }
}
