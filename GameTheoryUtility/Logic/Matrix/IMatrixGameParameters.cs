namespace GameTheoryUtility.Logic.Matrix;

public interface IMatrixGameParameters
{
    public bool IsInteger { get; }
    public int DecimalPrecision { get; }
    public bool IsPassiveSecondPlayer { get; }
    public int StrategiesCountA { get; }
    public int StrategiesCountB { get; }
}
