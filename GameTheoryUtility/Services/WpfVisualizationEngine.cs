using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic;
using GameTheoryUtility.Logic.Elements;
using GameTheoryUtility.Logic.Matrix;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using WpfMath.Controls;

namespace GameTheoryUtility.Services
{
    public class WpfVisualizationEngine(UIElementCollection canvas) : IVisualizationEngine
    {
        public void DrawVisual(Action<SKCanvas, SKSize> drawer)
        {
            var sk = new SKElement() { Margin = new Thickness(0, 10, 0, 10), HorizontalAlignment = HorizontalAlignment.Stretch };
            sk.SetBinding(SKElement.HeightProperty, new Binding(nameof(sk.ActualWidth)) { Source = sk });
            sk.PaintSurface += (o, e) =>
            {
                e.Surface.Canvas.Clear(SKColors.LightGray);
                drawer(e.Surface.Canvas, e.Info.Size);
            };
            canvas.Add(sk);
        }

        public void Matrix(IMatrix matrix) =>
             canvas.Add(new ReadonlyMatrixControl() { Margin = new Thickness(0, 10, 0, 10), HorizontalAlignment = HorizontalAlignment.Center, DataContext = matrix is ReadonlyMatrix<int> integer ? new ReadonlyMatrixViewModel(integer) : new ReadonlyMatrixViewModel((ReadonlyMatrix<double>)matrix, 2) });

        public void Paragraph(string text)
        {
            canvas.Add(new TextBlock { FontSize = 16, Margin = new Thickness(0, 5, 0, 5), Text = text, TextAlignment = TextAlignment.Justify, FontWeight = FontWeight.FromOpenTypeWeight(800), HorizontalAlignment = HorizontalAlignment.Center });
            //var tex = new TextBlock() { LineHeight = 25 };
            //tex.Inlines.Add(new Run() {  BaselineAlignment = BaselineAlignment.Center ,FontSize = 16, Text = "This is the text: " });
            //tex.Inlines.Add(new FormulaControl { Padding = new Thickness(0,7,0,0), VerticalAlignment = VerticalAlignment.Center, VerticalContentAlignment = VerticalAlignment.Center, FontSize = 16, SystemTextFontName = "Arial", Formula = "\\begin{pmatrix} a & b & c \\\\ d & e & f \\end{pmatrix}" });
            //canvas.Add(text); 
        }

        public void Header(string text)
        {
            canvas.Add(new TextBlock { FontSize = 25, Margin = new Thickness(0, 10, 0, 5), Text = text, FontWeight = FontWeight.FromOpenTypeWeight(800), HorizontalAlignment = HorizontalAlignment.Center });
        }

        public void RenderElement(IElement element)
        {
            var ui = RenderToUIElement(element);
            canvas.Add(ui);
            if (ui is TextBlock text)
                text.SetBinding(TextBlock.WidthProperty, new Binding(nameof(StackPanel.ActualWidth)) { Source = text.Parent });
        }

        UIElement RenderToUIElement(IElement element)
        {
            switch (element)
            {
                case Par paragraph:
                    switch (paragraph.Type)
                    {
                        case ParagraphType.Math:
                            return RenderToMathUiElement(paragraph);
                        case ParagraphType.Header1:
                            return new TextBlock()
                            {
                                FontWeight = FontWeight.FromOpenTypeWeight(800),
                                TextAlignment = TextAlignment.Center,
                                Margin = new Thickness(0, 16, 0, 14),
                                FontSize = 20,
                                TextWrapping = TextWrapping.Wrap,
                                Text = (paragraph.Elements.First() as Tx)!.Value
                            };
                        case ParagraphType.Header2:
                            return new TextBlock()
                            {
                                FontWeight = FontWeight.FromOpenTypeWeight(500),
                                TextAlignment = TextAlignment.Center,
                                Margin = new Thickness(0, 8, 0, 6),
                                FontSize = 18,
                                TextWrapping = TextWrapping.Wrap,
                                Text = (paragraph.Elements.First() as Tx)!.Value
                            };
                        case ParagraphType.Plain:
                            var txt = new TextBlock()
                            {
                                FontWeight = FontWeight.FromOpenTypeWeight(500),
                                Margin = new Thickness(0, 4, 0, 4),
                                TextWrapping = TextWrapping.Wrap,
                                TextAlignment = TextAlignment.Justify,
                                FontSize = 16
                            };
                            foreach (var parElement in paragraph.Elements)
                                txt.Inlines.Add(RenderToUIElement(parElement));
                            return txt;
                        default:
                            return null!;
                    }
                case Tx text:
                    return new TextBlock { Text = text.Value, TextWrapping = TextWrapping.Wrap, TextAlignment = TextAlignment.Justify };
                case Ls list:
                    var stack = new StackPanel { Orientation = Orientation.Vertical };
                    foreach(var listItem in list.Items)
                    {
                        var txt = new TextBlock { TextWrapping = TextWrapping.Wrap };
                        txt.Inlines.Add(new TextBlock() { Text = "•  ", RenderTransform = new TranslateTransform(0, 3) });
                        txt.Inlines.Add(RenderToUIElement(listItem));
                        stack.Children.Add(txt);
                    }
                    return stack;
                default:
                    return null!;
            }
        }

        FormulaControl RenderToMathUiElement(IElement element)
        {
            return new FormulaControl()
            {
                FontWeight = FontWeight.FromOpenTypeWeight(800),
                SystemTextFontName = "Arial",
                FontSize = 16,
                Margin = new Thickness(0, 5, 0, 5),
                Formula = $"{RenderToLaTex(element)}",
                HorizontalAlignment = HorizontalAlignment.Center,
            };
        }

        string RenderToLaTex(IElement element)
        {
            switch (element)
            {
                case Tx text:
                    return text.Value;
                case Par paragraph:
                    return string.Join("", paragraph.Elements.Select(RenderToLaTex));
                case Sub subscript:
                    return $"{RenderToLaTex(subscript.Main)}_{{{RenderToLaTex(subscript.Subscript)}}}";
                case Sup supscript:
                    return $"{RenderToLaTex(supscript.Main)}^{{{RenderToLaTex(supscript.Superscript)}}}";
                case Ovr over:
                    return $"{RenderToLaTex(over.Operation)}_{{{RenderToLaTex(over.Subscript)}}}{RenderToLaTex(over.Operand)}";
                case Mat matrix:
                    var tx = $"\\matrix{{{string.Join(" \\\\ ", Enumerable.Range(0, matrix.Rows).Select(i => string.Join(" & ", Enumerable.Range(0, matrix.Columns).Select(j => RenderToLaTex(matrix.Elements[i * matrix.Columns + j])))))}}}";
                    return tx;
                case Div division:
                    return $"\\frac{{{RenderToLaTex(division.Top)}}}{{{RenderToLaTex(division.Bottom)}}}";
                case Lcb lcb:
                    return $"\\left\\{{{RenderToLaTex(lcb.Element)}\\right.";
                default:
                    return null!;
            }
        }
    }
}
