using SkiaSharp;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace GameTheoryUtility.Logic.Visual;

public static class GraphExtensions
{
    public static Stream RenderGraph(this Graph graph, int width = 1024, int height = 640)
    {
        var info = new SKImageInfo(width, height);
        using var surface = SKSurface.Create(info);
        surface.Canvas.Clear(SKColors.White);
        surface.Canvas.DrawGraph(new(width, height), graph);
        using var img = surface.Snapshot();
        using var data = img.Encode(SKEncodedImageFormat.Jpeg, 100);
        var ms = new MemoryStream();
        data.SaveTo(ms);
        ms.Position = 0;
        return ms;
    }

    public static void DrawGraph(this SKCanvas canvas, SKSize canvasSize, Graph graph)
    {
        var map = new GraphFocusHelper(graph, canvasSize);
        using (var gridLine = new SKPaint()
        {
            StrokeWidth = .6f,
            IsAntialias = true,
            StrokeCap = SKStrokeCap.Square,
            Color = SKColors.DarkGray
        }) 
        {
            foreach (var y in map.GetOptimalGridY())
                canvas.DrawLine(0, y, (float)map.TargetWidth, y, gridLine);
            foreach (var x in map.GetOptimalGridX())
                canvas.DrawLine(x, 0, x, (float)map.TargetHeight, gridLine);
        }
        using (var axisLine = new SKPaint()
        {
            StrokeWidth = 2f,
            StrokeCap = SKStrokeCap.Square,
            Style = SKPaintStyle.StrokeAndFill,
            Color = SKColors.Black,
            IsAntialias = true
        })
        {
            canvas.DrawLine(map.X(0), 0, map.X(0), (float)map.TargetHeight, axisLine);
            canvas.DrawLine(0, map.Y(0), (float)map.TargetWidth, map.Y(0), axisLine);
            canvas.DrawCircle(map.X(0), map.Y(0), 3, axisLine);
        }
        foreach (var quadLine in graph.Lines.Where(x => x.SurfaceShadow != Line.Shadow.None))
        {
            var bond = map.ProjectedBoundary(quadLine);
            var md = ((bond.x1 + bond.x2) / 2, (bond.y1 + bond.y2) / 2);
            var dst = map.TargetWidth / map.Scaler + map.TargetHeight / map.Scaler;
            using var path = new SKPath();
            path.MoveTo(map.X(md.Item1 + quadLine.B * dst), map.Y(md.Item2 - quadLine.A * dst));
            if (quadLine.SurfaceShadow == Line.Shadow.Over)
            {
                path.LineTo(map.X(md.Item1 + quadLine.B * dst + quadLine.A * dst), map.Y(md.Item2 - quadLine.A * dst + quadLine.B * dst));
                path.LineTo(map.X(md.Item1 - quadLine.B * dst + quadLine.A * dst), map.Y(md.Item2 + quadLine.A * dst + quadLine.B * dst));
            }
            else
            {
                path.LineTo(map.X(md.Item1 + quadLine.B * dst - quadLine.A * dst), map.Y(md.Item2 - quadLine.A * dst - quadLine.B * dst));
                path.LineTo(map.X(md.Item1 - quadLine.B * dst - quadLine.A * dst), map.Y(md.Item2 + quadLine.A * dst - quadLine.B * dst));
            }
            path.LineTo(map.X(md.Item1 - quadLine.B * dst), map.Y(md.Item2 + quadLine.A * dst));
            path.Close();
            using var paint = new SKPaint()
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(quadLine.Color & 0x00FFFFFF | 0x45000000),
                IsStroke = false
            };
            canvas.DrawPath(path, paint);
        }
        //foreach (var quad in graph.Quads)
        //{
        //    using var path = new SKPath();
        //    path.MoveTo(map.X(quad.Points[0].X), map.Y(quad.Points[0].Y));
        //    foreach(var point in quad.Points)
        //        path.LineTo(map.X(point.X), map.Y(point.Y));
        //    path.Close();
        //    using var paint = new SKPaint()
        //    {
        //        Style = SKPaintStyle.Fill,
        //        Color = new SKColor(quad.Color),
        //        IsStroke = false
        //    };
        //    canvas.DrawPath(path, paint);
        //}
        foreach(var line in graph.Lines)
        {
            var bond = map.ProjectedBoundary(line);
            using var paint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                Color = new SKColor(line.Color),
                IsAntialias = true
            };
            canvas.DrawLine(map.X(bond.x1), map.Y(bond.y1), map.X(bond.x2), map.Y(bond.y2), paint);
        }
        if(graph.Field != null)
        {
            var Cbl = map.SourceWindowMinX * graph.Field.DirectionX + map.SourceWindowMinY * graph.Field.DirectionY;
            var Ctr = map.SourceWindowMaxX * graph.Field.DirectionX + map.SourceWindowMaxY * graph.Field.DirectionY;

            var (start, end) = Cbl > Ctr ? (Ctr, Cbl) : (Cbl, Ctr);

            for(double i = 0; i < 1; i += 1.0 / 20)
            {
                var line = new Line() { A = graph.Field.DirectionX, B = graph.Field.DirectionY, C = ((end - start) * i) + start };
                var bond = map.ProjectedBoundary(line);
                using var paint = new SKPaint()
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 15,
                    Color = new SKColor(graph.Field.ColorAgainst.ArgbInterpolate(graph.Field.ColorAlong, i)),
                    IsAntialias = true
                };
                canvas.DrawLine(map.X(bond.x1), map.Y(bond.y1), map.X(bond.x2), map.Y(bond.y2), paint);
            }
        }
        foreach (var point in graph.Points)
        {
            var skpoint = new SKPoint(map.X(point.X), map.Y(point.Y));
            using (var paint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = new SKColor(point.Color),
                StrokeWidth = 10,
                StrokeCap = SKStrokeCap.Round,
                IsAntialias = true
            })
                canvas.DrawPoint(skpoint, paint);
            if(point.Label != null)
            {
                using (var labelPaint = new SKPaint()
                {
                    Color = point.Color.ArgbInterpolate(0xFF000000, .6),
                    Style = SKPaintStyle.Fill,
                    IsAntialias = true
                })
                    canvas.DrawText(point.Label, new SKPoint(skpoint.X - 20, skpoint.Y - 14), SKTextAlign.Left, new SKFont(SKTypeface.FromFamilyName("Ariel", 800, 10, SKFontStyleSlant.Upright)) { Size = 10 }, labelPaint);
            }
        }
        var labels = graph.Lines.Where(x => x.Label != null).ToArray();
        var height = (labels.Length + 1) * 18 + 6;
        var width = 170; //map.TargetWidth * (GraphFocusHelper.BOUNDARY_SCALER - 1) * 0.9 / 2;
        using (var white = new SKPaint()
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Fill
        })
            canvas.DrawRect(0, 0, (float)width, height, white);
        int ly = 18;
        for (int i = 0; i < labels.Length; i++, ly += 18)
            using (var paint = new SKPaint()
            {
                Color = labels[i].Color.ArgbInterpolate(0xFF000000, .6),
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            })
                canvas.DrawText(labels[i].Label, new SKPoint(1, ly), SKTextAlign.Left, new SKFont(SKTypeface.FromFamilyName("Ariel", 800, 14, SKFontStyleSlant.Upright)) { Size = 14 }, paint);
        using (var paint = new SKPaint()
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        })
            canvas.DrawText($"1 : {map.SourceGridCellSize}", new SKPoint(1, ly), SKTextAlign.Left, new SKFont(SKTypeface.FromFamilyName("Ariel", 800, 14, SKFontStyleSlant.Upright)) { Size = 14 }, paint);
    }

    public static uint ArgbInterpolate(this uint start, uint end, double location)
    {
        byte sa = (byte)((start >> 24) & 0xFF);
        byte sr = (byte)((start >> 16) & 0xFF);
        byte sg = (byte)((start >> 8) & 0xFF);
        byte sb = (byte)(start & 0xFF);

        byte ea = (byte)((end >> 24) & 0xFF);
        byte er = (byte)((end >> 16) & 0xFF);
        byte eg = (byte)((end >> 8) & 0xFF);
        byte eb = (byte)(end & 0xFF);

        byte a = (byte)(sa + (ea - sa) * location);
        byte r = (byte)(sr + (er - sr) * location);
        byte g = (byte)(sg + (eg - sg) * location);
        byte b = (byte)(sb + (eb - sb) * location);

        return ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
    }
}
