using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Elements;
using GameTheoryUtility.Logic.Matrix;
using System.Text.Json.Serialization;
using ZenLib;
using Div = GameTheoryUtility.Logic.Elements.Div;

namespace GameTheoryUtility.Logic.Solvers;

public class LinearProgrammingSolver : ISolver
{
    [JsonIgnore]
    public int LogicalPriority => 2;
    [JsonIgnore]
    public string Name => "Зведення до задачі лінійного програмування (ЗЛП)";
    public bool SolveGraphically { get; set; } = true;
    public bool SolveSimplex { get; set; } = false;

    public SolverViewModel CreateViewModel(bool initialIsEnabled = false) => new(
        this,
        initialIsEnabled,
        new SolverOptionViewModel("Графічний метод", v => SolveGraphically = v, SolveGraphically),
        new SolverOptionViewModel("Симплекс метод", v => SolveSimplex = v, SolveSimplex));

    public void Solve(GameComputationInstance instance)
    {
        if(!SolveSimplex && !SolveGraphically) 
            return;
        instance.AddMethodHeader("Пошук розв'язку шляхом зведення до ЗЛП", "approach.lp");

        var pr = instance.Matrix!.ToReal().ToOptimizationProblem();


        if (SolveGraphically)
        {
            var graphical = pr;
            instance.AddStepHeader("Графічний метод");
            instance.AddParagraph("Розв'язок графічним методом є доцільним якщо кількість стратегій хоча б одного гравця рівна двом, це дозволяє звести задачу до ЗЛП із двома змінними, яку можливо зообразити на графіку.");
            instance.AddParagraph("Побудуємо модель прямої задачі для пошуку оптимальної мішаної стратегії гравця А, прийнявши заміну:");
            AddLpModel(instance, graphical);
            instance.AddParagraph($"Видно, що кількість змінних становить {graphical.VariablesCount}, отже пряму ЗЛП {(graphical.VariablesCount > 2 ? "не" : "")} доцільно вирішувати графічно");
            if(graphical.VariablesCount > 2)
            {
                instance.AddParagraph("Побудуємо двоїсту ЗЛП (пошук оптимальної мішаної стратегії для гравця В):");
                graphical = graphical.ConstructComplementary();
                AddLpModel(instance, graphical);
                instance.AddParagraph($"Двоїста ЗЛП має {graphical.VariablesCount}, отже її вирішення графічним методом {(graphical.VariablesCount > 2 ? "також є недоцільним" : "є доцільним")}");
                if (graphical.VariablesCount > 2)
                    goto nographical;        
            }
            var n = graphical.Naming;
            var solution = graphical.SolveGraphical();
            if (graphical.Type == OptimizationType.Minimize)
                instance.Results.OptimalMixedStrategyA = [solution.x1 / solution.f, solution.x2 / solution.f];
            else
                instance.Results.OptimalMixedStrategyB = [solution.x1 / solution.f, solution.x2 / solution.f];
            instance.ComputationSteps.Add(new GraphComputationStep(solution.graph));
            instance.AddParagraph($"Отже отримані значення:");
            var strx1 = solution.x1.ToString($"F{instance.Parameters!.DecimalPrecision}");
            var strx2 = solution.x2.ToString($"F{instance.Parameters.DecimalPrecision}");
            var nu = solution.f.ToString($"F{instance.Parameters!.DecimalPrecision}");
            instance.AddParagraph(new Par(new Sub(n.Variable, "1"), new Tx($" = {strx1}")) { Type = ParagraphType.Math });
            instance.AddParagraph(new Par(new Sub(n.Variable, "2"), new Tx($" = {strx2}")) { Type = ParagraphType.Math });
            instance.AddParagraph(new Par($"\\nu = {nu}") { Type = ParagraphType.Math });
            instance.AddParagraph("Проведемо зворотню конвертацію у ймовірності:");
            instance.AddParagraph(new Par(new Sub(n.Probability, "1"), new Tx(" = "), new Div(new Tx(strx1), new Tx(strx2)), new Tx($" = {instance.Parameters.FormatC(solution.x1 / solution.f)}")) { Type = ParagraphType.Math });
            instance.AddParagraph(new Par(new Sub(n.Probability, "2"), new Tx(" = "), new Div(new Tx(strx1), new Tx(strx2)), new Tx($" = {instance.Parameters.FormatC(solution.x2 / solution.f)}")) { Type = ParagraphType.Math });
            instance.AddParagraph("Знайдемо оптимальну мішану стратегію для іншого гравця. Для цього скористаймося з'язком двоїстих та прямих змінних, за яким добуток зміної на різницю відповідного двоїстого обмеження з його правою константою рівний нулю.");
            instance.AddParagraph("Таким чином, обмеження, що стають строгими нерівностами за даного розв'язку, означатимуть нульове значення відповідної двоїстою змінної");
            instance.AddParagraph("Підставимо розв'язки у нерівності");
            double[] eqs = new double[graphical.EquationsCount - graphical.VariablesCount];
            double[] other = Enumerable.Repeat(Double.NaN, eqs.Length).ToArray();


            instance.Results.OptimalMixedStrategyA = [solution.x1 / solution.f, solution.x2 / solution.f];
            instance.Results.OptimalMixedStrategyB = [solution.x1 / solution.f, solution.x2 / solution.f];

            //var dual = prb.ConstructComplementary();
            //var dvars = Enumerable.Range(0, dual.VariablesCount).Select(x => Zen.Symbolic<double>($"{x}")).ToArray();
            //Zen<double>[] equations = Enumerable.Range(0, prb.VariablesCount).Select(x => (Zen<double>)0).ToArray();
            //for(int i = 0; i < prb.EquationsCount - prb.VariablesCount; i++)
            //{
            //    eqs[i] = prb.Coefficients[i, 0] * solution.x1 + prb.Coefficients[i, 1] * solution.x2;
            //    if (double.Abs(eqs[i] - prb.Constants[i]) < 1e-9) {
            //        instance.AddParagraph(new Par($"{eqs[i].ToString($"F{instance.Parameters.DecimalPrecision}")} = {prb.Constants[i].ToString($"F{instance.Parameters.DecimalPrecision}")}") { Type = ParagraphType.Math });
            //        for (int e = 0; e < equations.Length; e++)
            //            equations[e] += dual.Coefficients[e, i] * dvars[i];
            //    } 
            //    else
            //    {
            //        other[i] = 0;
            //        instance.AddParagraph(new Par(new Tx($"{eqs[i].ToString($"F{instance.Parameters.DecimalPrecision}")} ≠ {prb.Constants[i].ToString($"F{instance.Parameters.DecimalPrecision}")} ⇒ "), new Sub($"{(prb.Type == OptimizationType.Minimize ? "y" : "x")}", $"{i + 1}"), new Tx(" = 0")) { Type = ParagraphType.Math })
            //    }
            //    f
            //}


        }

    nographical:

        if (SolveSimplex)
        {
            instance.AddStepHeader("Симплекс метод");
            instance.AddParagraph("Було проведено обчислення за імплементацією симплекс методу (GLOP):");
            var res = pr.SolveSimplex();
            instance.AddParagraph($"Пряма ЗЛП: {string.Join(", ", res.primal)} => {res.f}");
            instance.AddParagraph($"Двоїста ЗЛП: {string.Join(", ", res.dual)} => {res.f}");
        }
    }

    static void AddLpModel(GameComputationInstance instance, LinearProgrammingOptimizationProblem problem)
    {
        var n = problem.Naming;
        instance.AddParagraph(new Par(new Sub(n.Variable, n.Index), new Tx(" = "), new Div(new Sub(new Tx(n.Probability), new Tx(n.Index)), new Tx("\\nu"))) { Type = ParagraphType.Math });
        instance.AddParagraph($"Де {n.Probability} позначає ймовірність стратегії у векторі мішаної стратегії а 𝜈 - ціну гри");
        var constraints = new Mat(problem.EquationsCount, 1);
        for (int i = 0; i < problem.EquationsCount; i++)
        {
            var par = new Par(new Tx(instance.Parameters!.FormatI(problem.Coefficients[i, 0])), new Sub(n.Variable, "1"));
            for (int j = 1; j < problem.VariablesCount; j++)
            {
                par.Elements.Add(new Tx(double.Sign(problem.Coefficients[i, j]) < 0 ? " - " : " + "));
                par.Elements.Add(new Tx(instance.Parameters!.FormatI(problem.Coefficients[i, j])));
                par.Elements.Add(new Sub(n.Variable, $"{j + 1}"));
            }
            par.Elements.Add(new Tx($" {(problem.Signs[i] == InequalityType.LesserOrEqual ? "\\leq" : "\\geq")} {instance.Parameters!.FormatI(problem.Constants[i])}"));
            constraints.Elements.Add(par);
        }
        instance.AddParagraph(new Par(new Lcb(constraints)) { Type = ParagraphType.Math });
        var trg = new Par("F:  ") { Type = ParagraphType.Math };
        for (int i = 0; i < problem.VariablesCount; i++)
            trg.Elements.AddRange(new Tx("1"), new Sub(n.Variable, $"{i + 1}"), new Tx(" + "));
        trg.Elements.RemoveAt(trg.Elements.Count - 1);
        trg.Elements.Add(new Tx($" \\rightarrow {n.Goal}"));
        instance.AddParagraph(trg);
    }

    public bool IsApplicable(IMatrixGameParameters parameters) => !parameters.IsPassiveSecondPlayer;

    public ISolver Clone() => new LinearProgrammingSolver
    {
        SolveGraphically = SolveGraphically,
        SolveSimplex = SolveSimplex,
    };
}