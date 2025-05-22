using GameTheoryUtility.Logic.Matrix;
using GameTheoryUtility.Logic.Matrix.Representation;

namespace GameTheoryUtility.Controls;

public class MatrixViewModel
{
    public MatrixViewModel(Matrix<int> matrix)
    {
        Size = matrix.Size;
        List<CellViewModel> cells = [];
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                cells.Add(new CellViewModel(new IntegerValueFormatterProxy(matrix, i, j)));
        Cells = cells;
    }

    public MatrixViewModel(Matrix<double> matrix, int decimalPrecision)
    {
        Size = matrix.Size;
        List<CellViewModel> cells = [];
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                cells.Add(new CellViewModel(new DoubleValueFormatterProxy(matrix, i, j, decimalPrecision)));
        Cells = cells;
    }

    public IEnumerable<CellViewModel> Cells { get; }

    public MatrixSize Size { get; }
}
