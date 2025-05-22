using GameTheoryUtility.Logic.Elements;
using SkiaSharp;

namespace GameTheoryUtility.Logic;

public interface IVisualizationEngine
{
    public void RenderElement(IElement element);
    public void DrawVisual(Action<SKCanvas, SKSize> drawer);
}
