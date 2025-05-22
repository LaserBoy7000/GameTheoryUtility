using DocumentFormat.OpenXml.Drawing.Diagrams;
using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Contexts;
using GameTheoryUtility.Logic.Game;
using GameTheoryUtility.Logic.Matrix;
using GameTheoryUtility.Logic.Solvers;
using GameTheoryUtility.Logic.Visual;
using Microsoft.Office.Interop.Word;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Task = System.Threading.Tasks.Task;

namespace GameTheoryUtility.Logic.External.Word;

class WordFileExporter
{
    public string? FileType { get; set; }
    public bool ProducePdf { get; set; } = false;
    public bool AsMultipleFiles { get; set; } = false;
    public bool IncludeProblem { get; set; } = false;
    public bool IncludeSolution { get; set; } = false;

    string? _exportPath;
    string? _contextName;
    WordFileWriter? _globalFile;
    List<string> _createdWordDocuments = [];
    public Task ExportAsync(GenerationContext context, string exportPath) => Task.Run(() =>
    {
        _exportPath = Path.Combine(exportPath, context.Name!);
        _contextName = context.Name;
        if (Directory.Exists(_exportPath))
            Directory.Delete(_exportPath, true);
        Directory.CreateDirectory(_exportPath);

        for (int i = 0; i < context.Instances.Count; i++)
            WriteInstance(context.Instances[i], i + 1);

        _globalFile?.Save();
        _globalFile?.Dispose();

        if (ProducePdf)
            SaveAsPdf(_createdWordDocuments.ToArray());


        Process.Start("explorer.exe", exportPath);

        _exportPath = null;
        _contextName = null;
        _createdWordDocuments.Clear();
       
    });

    void WriteInstance(GameEnvironment instance, int index)
    {
        if (AsMultipleFiles)
        {
            using var file = CreateFileForInstance(index);
            WriteInstanceIntoDocument(file, instance, index);
        }
        else
            WriteInstanceIntoDocument(_globalFile == null ? CreateGlobalFile() : _globalFile, instance, index);
    }

    void WriteInstanceIntoDocument(WordFileWriter writer, GameEnvironment instance, int index)
    {
        writer.UseStyle(new DefaultEducationalDocument());
        writer.PageHeader($"Варіант {index}");
        if (IncludeProblem)
        {
            writer.CommonParagraph();
            writer.CommonText($"Задано платіжну матрицю для гри {(instance.Parameters.IsPassiveSecondPlayer ? "активного гравця проти природніх умов" : "двох гравців")}");
            if (instance.Matrix is Matrix<int> integer)
                writer.AddMatrix(integer);
            if (instance.Matrix is Matrix<double> real)
                writer.AddMatrix(real, instance.Parameters.DecimalPrecision);
            List<string> tasks = [];
            foreach(var solver in instance.Solvers)
                switch (solver)
                {
                    case CleanStrategySolver clean:
                        if (clean.FindMaxMinStrategy)
                            tasks.Add("максимінну стратегію");
                        if (clean.FindMinMaxStrategy)
                            tasks.Add("мінімаксну стратегію");
                        if (clean.FindSaddlePoint)
                            tasks.Add("сідлову точку або її відсутність");
                        break;
                    case LinearProgrammingSolver linear:
                        if (linear.SolveGraphically)
                            tasks.Add("розв'язок у мішаних стратегіях графічним методом");
                        if (linear.SolveSimplex)
                            tasks.Add("розв'язок у мішаних стратегіях симплекс методом");
                        break;
                    case BrownRobinsonSolver:
                        tasks.Add("розв'язок у мішаних стратегіях методом Брауна-Робінсона");
                        break;
                    case LagrangeCriterionSolver:
                        tasks.Add("оптимальну стратегію за критерієм Лагранжа");
                        break;
                    case HurwitzCriterionSolver:
                        tasks.Add("оптимальну стратегію за критерієм Гурвіца");
                        break;
                    case SavageCriterionSolver:
                        tasks.Add("оптимальну стратегію за критерієм Лагранжа");
                        break;
                    case WaldCriterionSolver:
                        tasks.Add("оптимальну стратегію за критерієм Вальда");
                        break;
                }
            writer.CommonText("Знайти:");
            writer.List(tasks.ToArray());
        }
        if (IncludeSolution)
        {
            writer.CommonParagraph();
            writer.CommonParagraph();
            writer.CommonText("Відповіді:");
            writer.CommonParagraph();
            if (instance.LastComputation!.Results.DominatedA != null)
            {
                writer.CommonParagraph();
                writer.CommonText($"Доміновані стратегії гравця А: {string.Join("; ", instance.LastComputation.Results.DominatedA.Select(x => $"{x.Item2} > {x.Item1}"))}");
            }
            if (instance.LastComputation.Results.DominatedB != null)
            {
                writer.CommonParagraph();
                writer.CommonText($"Доміновані стратегії гравця B: {string.Join("; ", instance.LastComputation.Results.DominatedA.Select(x => $"{x.Item2} > {x.Item1}"))}");
            }
            if (instance.Solvers.Any(x => x.LogicalPriority <= 1))
            {
                writer.CommonParagraph();
                writer.CommonText($"Максимін: {instance.LastComputation!.Results.OptimalCleanStrategyA}");
                writer.CommonParagraph();
                writer.CommonText($"Мінімакс: {instance.LastComputation!.Results.OptimalCleanStrategyB}");
                writer.CommonParagraph();
                writer.CommonText($"Сідлова точка: {(instance.LastComputation.Results.SaddlePoint == null ? "немає" : instance.Parameters.FormatI(instance.LastComputation.Results.SaddlePoint!.Value))}");
            }
            if (instance.Solvers.Any(x => x.LogicalPriority > 1))
            {
                writer.CommonParagraph();
                writer.CommonText($"Мішана стратегія А: [{string.Join("; ", instance.LastComputation!.Results.OptimalMixedStrategyA!.Select(instance.Parameters.FormatC))}]");
                writer.CommonParagraph();
                writer.CommonText($"Мішана стратегія В: [{string.Join("; ", instance.LastComputation!.Results.OptimalMixedStrategyB!.Select(instance.Parameters.FormatC))}]");
            }
            var graph = instance.LastComputation.ComputationSteps.FirstOrDefault(x => x is GraphComputationStep);
            if(graph != null)
            {
                writer.CommonParagraph();
                writer.CommonText($"Графічний розв'язок ЗЛП:");
                using var graphs = ((GraphComputationStep)graph).Graph.RenderGraph();
                writer.AddImage(graphs);
            }
        }
    }

    WordFileWriter CreateFileForInstance(int index)
    {
        var fileName = Path.Combine(_exportPath!, $"Варіант {index}.docx");
        _createdWordDocuments.Add(fileName);
        return new WordFileWriter(File.Open(fileName, FileMode.Create, FileAccess.ReadWrite));
    }

    WordFileWriter CreateGlobalFile()
    {
        var fileName = Path.Combine(_exportPath!, $"{(FileType == null ? "" : $"{FileType}_")}{_contextName!}.docx");
        _createdWordDocuments.Add(fileName);
        return _globalFile = new WordFileWriter(File.Open(fileName, FileMode.Create, FileAccess.ReadWrite));
    }
    
    void SaveAsPdf(params string[] wordFiles)
    {
         object misValue = System.Reflection.Missing.Value;

        var word = new Application();

        foreach (var wordFile in wordFiles)
        {
            Document doc = word.Documents.Open(wordFile);
            doc.Activate();

            doc.SaveAs2(wordFile.Replace(".docx", ".pdf"), WdSaveFormat.wdFormatPDF, misValue, misValue, misValue,
            misValue, misValue, misValue, misValue, misValue, misValue, misValue);

            doc.Close();
            releaseObject(doc);
        }

        word.Quit();
        releaseObject(word);

        static void releaseObject(object obj)
        {
            try
            {
                Marshal.ReleaseComObject(obj);
                obj = null!;
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}
