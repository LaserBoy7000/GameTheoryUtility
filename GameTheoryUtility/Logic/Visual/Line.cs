namespace GameTheoryUtility.Logic.Visual;

public class Line
{
    public string? Label { get; set; }
    public double A { get; set; }
    public double B { get; set; }
    public double C { get; set; }
    public uint Color { get; set; } = 0xFF000000;
    public Shadow SurfaceShadow { get; set; } = Shadow.None;
    public Style StrokeStyle { get; set; } = Style.Solid;

    public enum Shadow
    {
        None,
        Under,
        Over,
    }

    public enum Style
    {
        Dashed,
        Solid
    }

    public double X(double y) => A != 0 ? (C - B * y) / A : double.NaN;

    public double Y(double x) => B != 0 ? (C - A * x) / B : double.NaN;
}
