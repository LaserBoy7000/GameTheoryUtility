namespace GameTheoryUtility.Logic.Matrix;

public class UniqueMedianIntegerFiller
{
    readonly Matrix<int> _matrix;
    readonly GeneratedMatrixGameParameters _parameters;
    readonly Random _generator;
    readonly List<int> _left;
    readonly List<int> _right;
    int _i = int.MinValue;
    int _j = int.MinValue;

    public UniqueMedianIntegerFiller(GeneratedMatrixGameParameters parameters, Matrix<int> matrix)
    {
        _matrix = matrix;
        _parameters = parameters;
        _generator = new Random(parameters.GeneratorSeed!.Value);
        int max = (int)parameters.MaxCellValue + 1;
        int min = (int)double.Ceiling(parameters.MinCellValue);
        var source = Enumerable.Range(min, max - min).ToList();
        var quantity = matrix.Size.Columns * matrix.Size.Rows;
        if (quantity > source.Count)
            throw new InvalidOperationException();
        var available = new List<int>();
        for (int i = 0; i < quantity; i++)
        {
            var nx = _generator.Next(0, source.Count);
            available.Add(source[nx]);
            source.RemoveAt(nx);
        }
        available = available.OrderBy(x => x).ToList();
        _left = available.Slice(0, available.Count / 2);
        _right = available.Slice(available.Count / 2, available.Count - available.Count / 2);
    }

    public void Fill()
    {
        FillEmpty();
        ChooseRandomCell();
        _matrix[_i, _j] = TakeRight();
        if (_parameters.EnableSaddlePoint)
        {
            _parameters.DesirableSaddlePoint = _matrix[_i, _j];
            FillColumnWithLeft();
            _j = FillRowWithRight();
        }
        FillWithoutSaddlePoint();
    }

    void FillEmpty() => _matrix.Iterate((m, i, j) => m[i, j] = int.MinValue);

    int FillRowWithRight()
    {
        var last = int.MinValue;
        for (int j = 0; j < _matrix.Size.Columns; j++)
            if (j != _j)
            {
                last = j;
                _matrix[_i, j] = TakeRight();
            }
        return last;
    }

    int FillColumnWithLeft()
    {
        var last = int.MinValue;
        for (int i = 0; i < _matrix.Size.Rows; i++)
            if (i != _i)
            {
                last = i;
                _matrix[i, _i] = TakeLeft();
            }
        return last;
    }

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
                    _matrix[_i, _j] = TakeLeft();
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
                    _matrix[_i, _j] = TakeRight();
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
        for (int j = 0; j < _matrix.Size.Columns; j++)
            if (_matrix[_i, j] == int.MinValue)
                return;
        for (int i = 0; i < _matrix.Size.Rows; i++)
            if (_matrix[i, _j] == int.MinValue)
                return;

        for (int i = 0; i < _matrix.Size.Rows; i++)
            for (int j = 0; j < _matrix.Size.Columns; j++)
                if (_matrix[i, j] == int.MinValue)
                {
                    _j = j;
                    _i = i;
                    return;
                }
    }

    bool JumpToRandomEmptyInRow()
    {
        List<int> _empty = [];
        for (int j = 0; j < _matrix.Size.Columns; j++)
            if (_matrix[_i, j] == int.MinValue)
                _empty.Add(j);
        if (_empty.Count == 0)
            return false;
        _j = _empty[_generator.Next(0, _empty.Count)];
        return true;
    }

    bool JumpToRandomEmptyInColumn()
    {
        List<int> _empty = [];
        for (int i = 0; i < _matrix.Size.Rows; i++)
            if (_matrix[i, _j] == int.MinValue)
                _empty.Add(i);
        if (_empty.Count == 0)
            return false;
        _i = _empty[_generator.Next(0, _empty.Count)];
        return true;
    }

    void ChooseRandomCell()
    {
        _i = _generator.Next(0, _matrix.Size.Rows);
        _j = _generator.Next(0, _matrix.Size.Columns);
    }

    int TakeLeft()
    {
        int val = 0;
        if (_left.Any())
        {
            val = _left.Last();
            _left.RemoveAt(_left.Count - 1);
        }
        else
            val = TakeRight();
        return val;
    }

    int TakeRight()
    {
        int val = 0;
        if (_right.Any())
        {
            val = _right.Last();
            _right.RemoveAt(_right.Count - 1);
        }
        else
            val = TakeRight();
        return val;
    }

    bool IsFilled() => _right.Count == 0 && _left.Count == 0;
}
