namespace GameTheoryUtility.Logic.Matrix;

public interface IGeneratedMatrixGameParameters : IMatrixGameParameters
{
    public int? DesirableSaddlePoint { get; }
    public bool EnableSaddlePoint { get; }
    public double MaxCellValue { get; }
    public double MinCellValue { get; }
    public int? GeneratorSeed { get; }
    public bool NoRepeats { get; }
}
