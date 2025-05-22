using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using GameTheoryUtility.Logic.Matrix;
using System.IO;
using System.Numerics;
using M = DocumentFormat.OpenXml.Math;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;

namespace GameTheoryUtility.Logic.External.Word;

public class WordFileWriter : IDisposable
{
    readonly Stream _stream;
    readonly WordprocessingDocument _document;
    readonly MainDocumentPart _main;

    public WordFileWriter(Stream stream)
    {
        _stream = stream;
        _document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);
        _main = _document.AddMainDocumentPart();
        _main.Document = new Document() { Body = new Body() };
        _main.AddNewPart<StyleDefinitionsPart>().Styles = new Styles();
        _main.AddNewPart<NumberingDefinitionsPart>();
    }

    public WordFileWriter UseStyle(IWordDocumentStyle style)
    {
        style.Add(_main);
        return this;
    }

    public WordFileWriter PageHeader(string text)
    {
        var p = new Paragraph(
            new ParagraphProperties() { ParagraphStyleId = new ParagraphStyleId { Val = IWordDocumentStyle.HEADER } },
            new Run(new Text(text)));
        _main.Document.Body!.Append(p);
        return this;
    }

    Paragraph? _currentParagraph = null;
    public WordFileWriter CommonParagraph()
    {
        if (_currentParagraph != null)
            _main.Document.Body!.Append(_currentParagraph);
        _currentParagraph = new Paragraph(new ParagraphProperties(new ParagraphStyleId { Val = IWordDocumentStyle.NORMAL }));
        return this;
    }

    public WordFileWriter CommonText(string text)
    {
        if (_currentParagraph == null)
            _currentParagraph = new Paragraph(new ParagraphProperties(new ParagraphStyleId { Val = IWordDocumentStyle.NORMAL }));
        _currentParagraph.Append(new Run(new Text(text)));
        return this;
    }

    public WordFileWriter List(params string[] entries)
    {
        if (_currentParagraph != null)
        {
            _main.Document.Body!.Append(_currentParagraph);
            _currentParagraph = null;
        }
        foreach (var entry in entries)
            _main.Document.Body!.Append(new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId { Val = IWordDocumentStyle.NORMAL }),
                    new NumberingProperties(
                        new NumberingLevelReference() { Val = 0 },
                        new NumberingId() { Val = 1 }),
                    new Run(new Text(entry) { Space = SpaceProcessingModeValues.Preserve })));
        return this;
    }

    public WordFileWriter AddImage(Stream bitmap)
    {
        if (_currentParagraph != null)
        {
            _main.Document.Body!.Append(_currentParagraph);
            _currentParagraph = null;
        }

        var imagePart = _main.AddImagePart(ImagePartType.Jpeg);
        imagePart.FeedData(bitmap);

        var imagePartId = _main.GetIdOfPart(imagePart);

        long cx = 4100000L;
        long cy = 3200000L;

        var element = new Drawing(
            new Inline(
                new Extent() { Cx = cx, Cy = cy },
                new EffectExtent()
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                },
                new DocProperties()
                {
                    Id = (UInt32Value)1U,
                    Name = "Image"
                },
                new DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                    new GraphicFrameLocks() { NoChangeAspect = true }),
                new Graphic(
                    new GraphicData(
                        new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                            new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties()
                                {
                                    Id = (UInt32Value)0U,
                                    Name = "Image.jpg"
                                },
                                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()),
                            new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                new Blip()
                                {
                                    Embed = imagePartId,
                                    CompressionState = BlipCompressionValues.Print
                                },
                                new Stretch(new FillRectangle())),
                            new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                                new Transform2D(
                                    new Offset() { X = 0L, Y = 0L },
                                    new Extents() { Cx = cx, Cy = cy }),
                                new PresetGeometry(new AdjustValueList()) { Preset = ShapeTypeValues.Rectangle }))
                    )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
            )
            {
                DistanceFromTop = 0U,
                DistanceFromBottom = 0U,
                DistanceFromLeft = 0U,
                DistanceFromRight = 0U
            });

        var paragraph = new Paragraph(new Run(element));
        _main.Document.Body!.Append(paragraph);

        return this;
    }

    public WordFileWriter AddMatrix<T>(Matrix<T> matrix, int decimalPrecision = 2) where T : INumber<T>
    {
        if (_currentParagraph != null)
        {
            _main.Document.Body!.Append(_currentParagraph);
            _currentParagraph = null;
        }
        var mx = new M.Matrix(
                    new M.MatrixProperties(
                        new M.MatrixColumns(
                            new M.MatrixColumn(
                                new M.MatrixColumnProperties(
                                    new M.MatrixColumnCount() { Val = 3 },
                                    new M.MatrixColumnJustification() { Val = M.HorizontalAlignmentValues.Center })))));
        for (int i = 0; i < matrix.Size.Rows; i++)
        {
            var row = new M.MatrixRow();
            for (int j = 0; j < matrix.Size.Columns; j++)
            {
                var cell = new M.Base(
                    new M.Run(
                        new M.Text(matrix is Matrix<int> integer ? integer[i, j].ToString() : double.CreateChecked(matrix[i, j]).ToString($"F{decimalPrecision}"))
                    )
                );
                row.Append(cell);
            }
            mx.Append(row);
        }

        var math = new Paragraph(new M.Paragraph(new M.OfficeMath(mx)));
        _main.Document.Body!.Append(math);
        return this;
    }

    public void Save()
    {
        _document.Save();
    }

    public void Dispose()
    {
        _document.Dispose();
        _stream.Flush();
        _stream.Dispose();
    }
}
