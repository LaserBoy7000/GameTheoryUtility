using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Elements;
using GameTheoryUtility.Logic.Matrix;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Solvers;

public class CleanStrategySolver : ISolver
{
    [JsonIgnore]
    public int LogicalPriority => 1;
    [JsonIgnore]
    public string Name => "Вирішити у чистих стратегіях";
    public bool FindSaddlePoint { get; set; } = true;
    public bool FindMaxMinStrategy { get; set; } = true;
    public bool FindMinMaxStrategy { get; set; } = true;

    public SolverViewModel CreateViewModel(bool initialIsEnabled = false) => new(
        this,
        initialIsEnabled,
        new SolverOptionViewModel("Сідлова точка", v => FindSaddlePoint = v, FindSaddlePoint),
        new SolverOptionViewModel("Максимінна стратегія", v => FindMaxMinStrategy = v, FindMaxMinStrategy),
        new SolverOptionViewModel("Мінімаксна стратегія", v => FindMinMaxStrategy = v, FindMinMaxStrategy));

    public void Solve(GameComputationInstance instance)
    {
        var computeMx = instance.Matrix!.ToReal();

        if (FindSaddlePoint || FindMaxMinStrategy || FindMinMaxStrategy)
        {
            instance.AddMethodHeader("Пошук розв'язку у чистих стратегіях", "approach.clean");
            instance.AddParagraph($"Задано платіжну матрицю гри активних гравців:");
            instance.AddParagraph(new Par(instance.Matrix!.ToElement(instance.Parameters!.DecimalPrecision)) { Type = ParagraphType.Math });
            instance.AddParagraph(new Par("Із даної матриці видно, що:"));
            instance.AddParagraph(new Par(new Ls(
                    new Par("Постановлено гру двох гравців з нульовою сумою"),
                    new Par($"Кількість стратегій гравця A (рядки матриці) становить {instance.Matrix!.Size.Rows}"),
                    new Par($"Кількість стратегій гравця B (рядки матриці) становить {instance.Matrix!.Size.Columns}"))));
        }

        double arows = double.MinValue;
        int ai = -1;
        for(int i = 0; i < instance.Matrix.Size.Rows; i++)
        {
            double acols = double.MaxValue;
            for(int j = 0;  j < instance.Matrix.Size.Columns; j++)
                if (computeMx[i, j] < acols)
                    acols = computeMx[i, j];
            if (acols > arows)
            {
                ai = i + 1;
                arows = acols;
            }
        }
        instance.Results.OptimalCleanStrategyA = ai;

        if (FindMaxMinStrategy)
        {
            instance.AddStepHeader("Максимінна стратегія гравця А", "calc.maxmin");
            instance.AddParagraph("Суттю максимінної стратегії є максимізація виграшу гравця А за найнесприятливішого вибору гравця В:");
            instance.AddParagraph(new Par(new Sub("A", "i"), new Tx(" = "), new Ovr(new Tx("max"), new Ovr(new Tx("min"), new Sub("a", "ij"), new Tx("j")), new Tx("i"))) { Type = ParagraphType.Math });
            instance.AddParagraph("Для даної матриці максимінною стратегією є:");
            instance.AddParagraph(new Par(new Sub("A", "i"), new Tx($" = "), new Sub("A", ai.ToString())) { Type = ParagraphType.Math });
            instance.AddParagraph($"При цьому виплата гравцеві А (нижня ціна гри) становить {arows}.");
        }

        double bcols = double.MaxValue;
        int bj = -1;
        for (int j = 0; j < instance.Matrix.Size.Columns; j++)
        {
            double brows = double.MinValue;
            for (int i = 0; i < instance.Matrix.Size.Rows; i++)
                if (computeMx[i, j] > brows)
                    brows = computeMx[i, j];
            if (brows < bcols)
            {
                bj = j + 1;
                bcols = brows;
            }
        }
        instance.Results.OptimalCleanStrategyB = bj;

        if (FindMinMaxStrategy)
        {
            instance.AddStepHeader("Мінімаксна стратегія гравця B", "calc.minmax");
            instance.AddParagraph(new Par("Суттю мінімаксної стратегії є мінімізація збитку гравця B за найнесприятливішого вибору гравця А:"));
            instance.AddParagraph(new Par(new Sub("B", "j"), new Tx(" = "), new Ovr(new Tx("min"), new Ovr(new Tx("max"), new Sub("a", "ij"), new Tx("i")), new Tx("j"))) { Type = ParagraphType.Math });
            instance.AddParagraph("Для даної матриці максимінною стратегією є ");
            instance.AddParagraph(new Par(new Sub("B", "i"), new Tx($" = "), new Sub("A", ai.ToString())) { Type = ParagraphType.Math });
            instance.AddParagraph($"При цьому програш гравця В (верхня ціна гри) становить {bcols}");
        }

        if (arows == bcols)
        {
            instance.Results.SaddlePoint = arows;
            instance.Results.GameCost = arows;
        }

        if (FindSaddlePoint)
        {
            instance.AddStepHeader("Визначення сідлової точки", "calc.saddle");
            instance.AddParagraph(new Par("Сідлова точка платіжної матриці називається ціною гри. Вона являє собою задовільну вартість для обох гравців, відповідно обоє обиратимуть саме її. Ціна гри інує якщо верхня та нижня ціни рівні:"));
            instance.AddParagraph(new Par("\\nu = \\alpha = \\beta") { Type = ParagraphType.Math });
            instance.AddParagraph($"Для даної гри {arows.ToString($"F{instance.Parameters!.DecimalPrecision}")} {(arows == bcols ? "=" : "≠")} {bcols.ToString($"F{instance.Parameters!.DecimalPrecision}")}{(arows == bcols ? ", тому дане значення є ціною гри та сідловою точкою" : ", отже розв'язок у чистих стратегіях відсутній")}");
        }
    }

    public bool IsApplicable(IMatrixGameParameters parameters) => !parameters.IsPassiveSecondPlayer;

    public ISolver Clone() => new CleanStrategySolver() 
    { 
        FindMaxMinStrategy =  FindMaxMinStrategy, 
        FindMinMaxStrategy = FindMinMaxStrategy, 
        FindSaddlePoint = FindSaddlePoint 
    };
}
