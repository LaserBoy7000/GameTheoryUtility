namespace GameTheoryUtility.Logic.Matrix;

public class MatrixGameParameters : IMatrixGameParameters
{
    public bool IsInteger { get; set; } = true;
    public int DecimalPrecision { get; set; } = 2;
    public bool IsPassiveSecondPlayer { get; set; } = false;
    public int StrategiesCountA { get; set; } = 3;
    public int StrategiesCountB { get; set; } = 3;

    protected MatrixGameParameters Transfer(MatrixGameParameters other)
    {
        other.IsInteger = IsInteger;
        other.DecimalPrecision = DecimalPrecision;
        other.IsPassiveSecondPlayer = IsPassiveSecondPlayer;
        other.StrategiesCountA = StrategiesCountA;
        other.StrategiesCountB = StrategiesCountB;
        return other;
    }

    public MatrixGameParameters Clone() => Transfer(new()); 
}
