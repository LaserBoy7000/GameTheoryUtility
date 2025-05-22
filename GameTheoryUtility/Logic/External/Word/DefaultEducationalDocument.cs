using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace GameTheoryUtility.Logic.External.Word;

public class DefaultEducationalDocument : IWordDocumentStyle
{
    public string Name => "Типовий документ ВНЗ (Times New Roman 14pt)";

    public void Add(MainDocumentPart main)
    {
        AddList(main.NumberingDefinitionsPart!);
        AddNormal(main.StyleDefinitionsPart!.Styles!);
        AddHeader(main.StyleDefinitionsPart!.Styles!);
    }

    static void AddList(NumberingDefinitionsPart numbering)
    {
        Numbering element =
        new Numbering(
            new AbstractNum(
                new Level(
                new NumberingFormat() { Val = NumberFormatValues.Bullet },
                new LevelText() { Val = "•" }
            )
                { LevelIndex = 0 }
        )
            { AbstractNumberId = 1 },
        new NumberingInstance(
            new AbstractNumId() { Val = 1 }
        )
        { NumberID = 1 });

        element.Save(numbering);
    }

    static void AddNormal(Styles styleList)
    {
        Style style = new Style()
        {
            Type = StyleValues.Paragraph,
            StyleId = IWordDocumentStyle.NORMAL,
            CustomStyle = true
        };
        StyleName styleName1 = new StyleName() { Val = "Звичайний" };
        BasedOn basedOn1 = new BasedOn() { Val = "Normal" };
        NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() { Val = "Normal" };
        StyleParagraphProperties paragraphProperties1 = new StyleParagraphProperties();
        paragraphProperties1.Append(new Tabs(
            new TabStop
            {
                Val = TabStopValues.Left,
                Position = 300
            }
        ));
        paragraphProperties1.Append(new Justification() { Val = JustificationValues.Both });
        style.Append(styleName1);
        style.Append(basedOn1);
        style.Append(paragraphProperties1);
        style.Append(nextParagraphStyle1);

        // Create the StyleRunProperties object and specify some of the run properties.
        StyleRunProperties styleRunProperties1 = new StyleRunProperties();
        Color color1 = new Color() { Val = "000000" };
        RunFonts font1 = new RunFonts() { HighAnsi = "Times New Roman", EastAsia = "Ariel", Ascii = "Times New Roman", ComplexScript = "Courier New" };
        // Specify a 12 point size.
        FontSize fontSize1 = new FontSize() { Val = "28" };
        styleRunProperties1.Append(color1);
        styleRunProperties1.Append(font1);
        styleRunProperties1.Append(fontSize1);

        // Add the run properties to the style.
        style.Append(styleRunProperties1);

        styleList.Append(style);
    }

    static void AddHeader(Styles styleList)
    {
        Style style = new Style()
        {
            Type = StyleValues.Paragraph,
            StyleId = IWordDocumentStyle.HEADER,
            CustomStyle = true
        };
        StyleName styleName1 = new StyleName() { Val = "Заголовок" };
        BasedOn basedOn1 = new BasedOn() { Val = "Normal" };
        NextParagraphStyle nextParagraphStyle1 = new NextParagraphStyle() { Val = "Normal" };
        StyleParagraphProperties paragraphProperties1 = new StyleParagraphProperties();
        paragraphProperties1.Append(new Justification() { Val = JustificationValues.Center });
        paragraphProperties1.Append(new PageBreakBefore());
        style.Append(styleName1);
        style.Append(basedOn1);
        style.Append(paragraphProperties1);
        style.Append(nextParagraphStyle1);

        // Create the StyleRunProperties object and specify some of the run properties.
        StyleRunProperties styleRunProperties1 = new StyleRunProperties();
        Bold bold1 = new Bold();
        Color color1 = new Color() { Val = "000000" };
        RunFonts font1 = new RunFonts() { HighAnsi = "Times New Roman", EastAsia = "Ariel", Ascii = "Times New Roman", ComplexScript = "Courier New" };
        Italic italic1 = new Italic();
        // Specify a 12 point size.
        FontSize fontSize1 = new FontSize() { Val = "28" };
        styleRunProperties1.Append(bold1);
        styleRunProperties1.Append(color1);
        styleRunProperties1.Append(font1);
        styleRunProperties1.Append(fontSize1);
        styleRunProperties1.Append(italic1);

        // Add the run properties to the style.
        style.Append(styleRunProperties1);

        styleList.Append(style);
    }
}
