namespace GameTheoryUtility.Logic.Matrix;

public sealed class GeneratedMatrixGameParameters : MatrixGameParameters, IGeneratedMatrixGameParameters
{
    public int? DesirableSaddlePoint { get; set; }
    public double MaxCellValue { get; set; } = 10;
    public double MinCellValue { get; set; } = 0;
    public int? GeneratorSeed { get; set; }
    public bool NoRepeats { get; set; } = false;
    public bool EnableSaddlePoint { get; set; } = false;

    public new GeneratedMatrixGameParameters Clone()
    {
        var clone = new GeneratedMatrixGameParameters()
        {
            DesirableSaddlePoint = DesirableSaddlePoint,
            MaxCellValue = MaxCellValue,
            MinCellValue = MinCellValue,
            GeneratorSeed = GeneratorSeed,
            NoRepeats = NoRepeats,
            EnableSaddlePoint = EnableSaddlePoint
        };
        Transfer(clone);
        return clone;
    }
}