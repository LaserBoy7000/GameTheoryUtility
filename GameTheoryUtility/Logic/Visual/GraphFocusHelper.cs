using SkiaSharp;

namespace GameTheoryUtility.Logic.Visual;

public class GraphFocusHelper
{
    public const double BOUNDARY_SCALER = 1.5;

    readonly double _offsetX;
    readonly double _offsetY;

    public GraphFocusHelper(Graph graph, SKSize canvasSize)
    {
        double miny = graph.Points.Min(x => x.Y),
               maxy = graph.Points.Max(x => x.Y),
               minx = graph.Points.Min(y => y.X),
               maxx = graph.Points.Max(y => y.X);

        SourceOriginX = (maxx + minx) / 2;
        SourceOriginY = (maxy + miny) / 2;
        var width = maxx - minx;
        var height = maxy - miny;
        SourceWidth = width * BOUNDARY_SCALER;
        SourceHeight = height * BOUNDARY_SCALER;
        SourceMinX = SourceOriginX - SourceWidth / 2;
        SourceMinY = SourceOriginY - SourceHeight / 2;
        TargetHeight = canvasSize.Height;
        TargetWidth = canvasSize.Width;
      
        SourceGridCellSize = double.Pow(10, double.Floor(double.Log10(double.Min(width, height) / 5)));

        if (double.Abs(SourceWidth - canvasSize.Width) < double.Abs(SourceHeight - canvasSize.Height))
            Scaler = canvasSize.Width / SourceWidth;
        else
            Scaler = canvasSize.Height / SourceHeight;

        SourceWindowMinX = SourceOriginX - TargetWidth / Scaler / 2;
        SourceWindowMinY = SourceOriginY - TargetHeight / Scaler / 2;
        SourceWindowMaxX = SourceWindowMinX + TargetWidth / Scaler;
        SourceWindowMaxY = SourceWindowMinY + TargetHeight / Scaler;

        _offsetX = canvasSize.Width / 2 / Scaler - SourceOriginX;
        _offsetY = canvasSize.Height / 2 / Scaler - SourceOriginY;
    }

    public double Scaler { get; }
    public double SourceOriginX { get; }
    public double SourceOriginY { get; }
    public double SourceMinX { get; }
    public double SourceMinY { get; }
    public double SourceWindowMinX { get; }
    public double SourceWindowMinY { get; }
    public double SourceWindowMaxX { get; }
    public double SourceWindowMaxY { get; }
    public double SourceWidth { get; }
    public double SourceHeight { get; }
    public double SourceGridCellSize { get; }
    public double TargetWidth { get; }
    public double TargetHeight { get; }

    public float X(double sourceX) => (float)((sourceX + _offsetX) * Scaler);
    public float Y(double sourceY) => (float)(TargetHeight - ((sourceY + _offsetY) * Scaler));

    public IEnumerable<float> GetOptimalGridY()
    {
        var start = double.Floor(SourceWindowMinY / SourceGridCellSize) * SourceGridCellSize;
        var end = SourceWindowMaxY + SourceGridCellSize;
        for(double i = start; i < end; i += SourceGridCellSize)
            yield return Y(i);
    }

    public IEnumerable<float> GetOptimalGridX()
    {
        var start = double.Floor(SourceWindowMinX / SourceGridCellSize) * SourceGridCellSize;
        var end = SourceWindowMaxX + SourceGridCellSize;
        for (double i = start; i < end; i += SourceGridCellSize)
            yield return X(i);
    }

    public (double x1, double y1, double x2, double y2) ProjectedBoundary(Line line)
    {
        var bottom = line.X(SourceWindowMinY);
        if (double.IsNaN(bottom))
            return (SourceWindowMinX, line.Y(0), SourceWindowMaxX, line.Y(0));
        var left = double.Clamp(line.Y(SourceWindowMinX), SourceWindowMinY, SourceWindowMaxY);
        if(double.IsNaN(left))
            return (line.X(0), SourceWindowMinY, line.X(0), SourceWindowMaxY);
        var top = line.X(SourceWindowMaxY);    
        var right = double.Clamp(line.Y(SourceWindowMaxX), SourceWindowMinY, SourceWindowMaxY);

        if (bottom < SourceWindowMinX)
            return (SourceWindowMinX, left, double.Clamp(top, SourceWindowMinX, SourceWindowMaxX), right);
        if (bottom > SourceWindowMaxX)
            return (SourceWindowMaxX, right, double.Clamp(top, SourceWindowMinX, SourceWindowMaxX), left);
        if (top < SourceWindowMinX)
            return (SourceWindowMinX, left, bottom, SourceWindowMinY);
        if (top > SourceWindowMaxX)
            return (SourceWindowMaxX, right, bottom, SourceWindowMinY);
        return (top, SourceWindowMaxY, bottom, SourceWindowMinY);
    }
}
