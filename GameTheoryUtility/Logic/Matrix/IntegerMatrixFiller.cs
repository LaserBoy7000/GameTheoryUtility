namespace GameTheoryUtility.Logic.Matrix;

public class IntegerMatrixFiller(GeneratedMatrixGameParameters parameters, Matrix<int> matrix)
{
    readonly Random _generator = new(parameters.GeneratorSeed!.Value);
    readonly int _max = (int)parameters.MaxCellValue + 1;
    readonly int _min = (int)double.Ceiling(parameters.MinCellValue);
    int _i = int.MinValue;
    int _j = int.MinValue;
    int _last = int.MinValue;

    public void Fill()
    {
        FillEmpty();
        if (parameters.EnableSaddlePoint)
        {
            if (parameters.DesirableSaddlePoint == null)
                parameters.DesirableSaddlePoint = (_max + _min) / 2;
            FillWithSaddlePoint(parameters.DesirableSaddlePoint.Value);
        }
        else
        {
            ChooseRandomCell();
            FillRandomValue();
            FillWithoutSaddlePoint();
        }
    }

    void FillEmpty() => matrix.Iterate((m, i, j) => m[i, j] = int.MinValue);

    void FillWithoutSaddlePoint()
    {
        bool inRow = true;
        while (true)
        {
            if (inRow)
            {
                inRow = false;
                if (JumpToRandomEmptyInRow())
                {
                    matrix[_i, _j] = SmallerOrEqual();
                    continue;
                }
                if (IsFilled())
                    return;
                continue;
            }
            else
            {
                inRow = true;
                if (JumpToRandomEmptyInColumn())
                {
                    matrix[_i, _j] = LargerOrEqual();
                    continue;
                }
                if (IsFilled())
                    return;
                else
                    EscapeIfJailed();
            }
        }
    }

    void EscapeIfJailed()
    {
        for(int j = 0; j < matrix.Size.Columns; j++)
            if (matrix[_i, j] == int.MinValue)
                return;
        for (int i = 0; i < matrix.Size.Rows; i++)
            if (matrix[i, _j] == int.MinValue)
                return;

        for(int i = 0; i < matrix.Size.Rows; i++)
            for(int j = 0; j < matrix.Size.Columns; j++)
                if (matrix[i, j] == int.MinValue)
                {
                    _j = j;
                    _i = i;
                    return;
                }
    }

    void FillWithSaddlePoint(int saddlePoint)
    {
        ChooseRandomCell();
        FillWithSaddlePointValue(saddlePoint);
        SaveCurrentValue();
        FillColumnWithSmallerValues();
        _j = FillRowWithLargerValues();
        FillWithoutSaddlePoint();
    }

    void ChooseRandomCell()
    {
        _i = _generator.Next(0, matrix.Size.Rows);
        _j = _generator.Next(0, matrix.Size.Columns);
    }

    void FillWithSaddlePointValue(int saddlePoint) => matrix[_i, _j] = saddlePoint;

    void FillRandomValue() =>
         matrix[_i, _j] = _generator.Next(_min, _max);

    void SaveCurrentValue() => _last = matrix[_i, _j];

    int SmallerOrEqual() => _generator.Next(_min, int.Max(_min + 1, _last));

    int LargerOrEqual() => _generator.Next(int.Min(_max - 1, _last + 1), _max);

    bool JumpToRandomEmptyInRow()
    {
        List<int> _empty = [];
        for(int j = 0; j < matrix.Size.Columns; j++)
            if(matrix[_i, j] == int.MinValue)
                _empty.Add(j);
        if(_empty.Count == 0)
            return false;
        SaveCurrentValue();
        _j = _empty[_generator.Next(0, _empty.Count)];
        return true;
    }

    int FillRowWithLargerValues()
    {
        var last = int.MinValue;
        for (int j = 0; j < matrix.Size.Columns; j++)
            if (j != _j)
            {
                last = j;
                matrix[_i, j] = LargerOrEqual();
            }
        return last;
    }

    int FillColumnWithSmallerValues()
    {
        var last = int.MinValue;
        for (int i = 0; i < matrix.Size.Rows; i++)
            if (i != _i)
            {
                last = i;
                matrix[i, _i] = SmallerOrEqual();
            }
        return last;
    }

    bool JumpToRandomEmptyInColumn()
    {
        List<int> _empty = [];
        for (int i = 0; i < matrix.Size.Rows; i++)
            if (matrix[i, _j] == int.MinValue)
                _empty.Add(i);
        if (_empty.Count == 0)
            return false;
        SaveCurrentValue();
        _i = _empty[_generator.Next(0, _empty.Count)];
        return true;
    }

    bool IsFilled()
    {
        for (int i = 0; i < matrix.Size.Rows; i++)
            for (int j = 0; j < matrix.Size.Columns; j++)
                if (matrix[i, j] == int.MinValue)
                    return false;
        return true;
    }
}