using GameTheoryUtility.Logic.Visual;
using Google.OrTools.LinearSolver;

namespace GameTheoryUtility.Logic.Solvers;

public struct LinearProblemNamingNotation
{

    public readonly static LinearProblemNamingNotation PRIMAL = new()
    {
        Variable = "x",
        Index = "i",
        Goal = "min",
        Probability = "p"
    };

    public readonly static LinearProblemNamingNotation DUAL = new()
    {
        Variable = "y",
        Index = "j",
        Goal = "max",
        Probability = "q"
    };

    public string Variable { get; init; }
    public string Index { get; init; }
    public string Goal { get; init; }
    public string Probability { get; init; } 
}

public class LinearProgrammingOptimizationProblem(double[,] coefficients, InequalityType[] signs, double[] constants, double[] targetFunctionCoefficients, OptimizationType type, int visualizationDecimalPrecision = 2)
{
    public int EquationsCount => coefficients.GetLength(0);
    public int VariablesCount => coefficients.GetLength(1);
    public int DualEquations => VariablesCount;
    public int DualVariables => EquationsCount - VariablesCount;
    public double[,] Coefficients => coefficients;
    public InequalityType[] Signs => signs;
    public double[] Constants => constants;
    public OptimizationType Type => type;
    public LinearProblemNamingNotation Naming { get; } = type == OptimizationType.Minimize ? LinearProblemNamingNotation.PRIMAL : LinearProblemNamingNotation.DUAL;

    public (Graph graph, double x1, double x2, double f) SolveGraphical()
    {
        List<(double x, double y)> satisfactory = [];
        List<(double x, double y)> notSatisfactory = [];
        for (int i = 0; i < EquationsCount - 1; i++)
            for (int j = i + 1; j < EquationsCount; j++)
            {
                // Extract coefficients for line i
                double a1 = coefficients[i, 0];
                double b1 = coefficients[i, 1];
                double c1 = constants[i];

                // Extract coefficients for line j
                double a2 = coefficients[j, 0];
                double b2 = coefficients[j, 1];
                double c2 = constants[j];

                // Compute determinant
                double det = a1 * b2 - a2 * b1;
                if (Math.Abs(det) < 1e-10)
                    continue; // Lines are parallel or coincident, skip

                // Cramer's Rule
                double x = (c1 * b2 - c2 * b1) / det;
                double y = (a1 * c2 - a2 * c1) / det;

                bool satisfiesAll = true;
                for (int k = 0; k < EquationsCount; k++)
                {
                    double lhs = coefficients[k, 0] * x + coefficients[k, 1] * y;
                    double rhs = constants[k];
                    bool valid = signs[k] switch
                    {
                        InequalityType.LesserOrEqual => lhs <= rhs + 1e-8,
                        InequalityType.GreaterOrEqual => lhs >= rhs - 1e-8,
                        _ => false
                    };
                    if (!valid)
                    {
                        satisfiesAll = false;
                        break;
                    }
                }

                if (satisfiesAll)
                    satisfactory.Add((x, y));
                else
                    notSatisfactory.Add((x, y));
            }
        var visual = new Graph();
        var variable = type == OptimizationType.Minimize ? "x" : "y";
        var randomizer = new Random(DateTime.Now.GetHashCode());
        for (int i = 0; i < EquationsCount; i++)
        {
            if (!((coefficients[i,0] == 0 || coefficients[i,1] == 0) && constants[i] == 0))
                visual.Lines.Add(new() { Label = $"{coefficients[i, 0].ToString($"F{visualizationDecimalPrecision}")}{variable}₁{(double.Sign(coefficients[i, 1]) >= 0 ? " + " : " - ")}{double.Abs(coefficients[i, 1]).ToString($"F{visualizationDecimalPrecision}")}{variable}₂ {(signs[i] == InequalityType.LesserOrEqual ? "≤" : "≥")} {constants[i].ToString($"F{visualizationDecimalPrecision}")}", Color = GenerateRandomColor(randomizer), A = coefficients[i, 0], B = coefficients[i, 1], C = constants[i], SurfaceShadow = signs[i] == InequalityType.LesserOrEqual ? Line.Shadow.Under : Line.Shadow.Over });
        }
        foreach (var ns in notSatisfactory)
            visual.Points.Add(new Point { X = ns.x, Y = ns.y, Color = 0xFF2d1a47, Label = $"({ns.x.ToString($"F{visualizationDecimalPrecision}")}; {ns.y.ToString($"F{visualizationDecimalPrecision}")})" });
        foreach (var s in satisfactory)
            visual.Points.Add(new Point { X = s.x, Y = s.y, Color = 0xFFdb530f, Label = $"({s.x.ToString($"F{visualizationDecimalPrecision}")}; {s.y.ToString($"F{visualizationDecimalPrecision}")})" });
        visual.Field = new Field { DirectionX = targetFunctionCoefficients[0], DirectionY = targetFunctionCoefficients[1] };

        (double x, double y) result = default;
        double f = default;
        try
        {
            result = type == OptimizationType.Minimize ? satisfactory.MinBy(x => targetFunctionCoefficients[0] * x.x + targetFunctionCoefficients[1] * x.y) : satisfactory.MaxBy(x => targetFunctionCoefficients[0] * x.x + targetFunctionCoefficients[1] * x.y);
            f = targetFunctionCoefficients[0] * result.x + targetFunctionCoefficients[1] * result.y;
        }
        catch
        {
            throw new Exception("Неможливо знайти перетин обмежень ЗЛП");
        }
        visual.Points.RemoveAll(x => x.X == result.x && x.Y == result.y);
        visual.Points.Add(new Point { X = result.x, Y = result.y, Color = 0xFFFF0000, Label = $"{(signs[0] == InequalityType.GreaterOrEqual ? "MIN" : "MAX")} ({result.x.ToString($"F{visualizationDecimalPrecision}")}; {result.y.ToString($"F{visualizationDecimalPrecision}")})" });

        return (visual, result.x, result.y, f);
    }

    public static uint GenerateRandomColor(Random rng)
    {
        byte r = (byte)rng.Next(50, 150);
        byte g = (byte)rng.Next(50, 150);
        byte b = (byte)rng.Next(50, 150);
        byte a = 255; // fully opaque

        return (uint)((a << 24) | (r << 16) | (g << 8) | b);
    }

    public (double[] primal, double[] dual, double f) SolveSimplex()
    {
        Solver solver = Solver.CreateSolver("GLOP");
        if (solver == null)
            throw new InvalidOperationException("Failed to create solver.");

        // Create variables
        Variable[] vars = new Variable[VariablesCount];
        for (int j = 0; j < VariablesCount; j++)
            vars[j] = solver.MakeNumVar(0.0, double.PositiveInfinity, $"x{j + 1}");

        List<Constraint> constraints = [];
        for (int i = 0; i < EquationsCount - VariablesCount; i++)
        {
            LinearExpr expr = new();
            for (int j = 0; j < VariablesCount; j++)
                expr += coefficients[i, j] * vars[j];

            switch (signs[i])
            {
                case InequalityType.LesserOrEqual:
                    constraints.Add(solver.Add(expr <= constants[i]));
                    break;
                case InequalityType.GreaterOrEqual:
                    constraints.Add(solver.Add(expr >= constants[i]));
                    break;
                default:
                    throw new InvalidOperationException("Unsupported inequality type.");
            }
        }

        // Define objective function
        Objective objective = solver.Objective();
        for (int j = 0; j < VariablesCount; j++)
            objective.SetCoefficient(vars[j], targetFunctionCoefficients[j]);

        objective.SetMinimization();
        if (type == OptimizationType.Maximize)
            objective.SetMaximization();

        // Solve the problem
        Solver.ResultStatus resultStatus = solver.Solve();

        if (resultStatus != Solver.ResultStatus.OPTIMAL)
            throw new InvalidOperationException("Не вдалося оптимізувати задачу симплекс методом.");

        // Extract solution
        double[] solution = new double[VariablesCount];
        for (int j = 0; j < VariablesCount; j++)
            solution[j] = vars[j].SolutionValue();
        double[] dual = new double[EquationsCount - VariablesCount];
        for(int i = 0; i < constraints.Count; i++)
            dual[i] = constraints[i].DualValue();

        double optimalValue = solver.Objective().Value();

        return (solution, dual, optimalValue);
    }

    public LinearProgrammingOptimizationProblem ConstructComplementary()
    {
        var coefficients2 = new double[DualEquations + DualVariables, DualVariables];
        for (var j = 0; j < VariablesCount; j++)
            for (var i = 0; i < DualVariables; i++)
                coefficients2[j, i] = coefficients[i, j];
        for (var i = 0; i < coefficients2.GetLength(1); i++)
            coefficients2[i + DualEquations, i] = 1;
        var equation2 = Enumerable.Repeat(1.0, coefficients2.GetLength(1)).ToArray();
        var side2 = Enumerable.Repeat(1.0, DualEquations).Concat(Enumerable.Repeat(0.0, DualVariables)).ToArray();
        var signs2 = Enumerable.Repeat(signs[0] == InequalityType.GreaterOrEqual ? InequalityType.LesserOrEqual : InequalityType.GreaterOrEqual, VariablesCount).Concat(Enumerable.Repeat(InequalityType.GreaterOrEqual, coefficients2.GetLength(1))).ToArray();
        return new (coefficients2, signs2, side2, equation2, type == OptimizationType.Maximize ? OptimizationType.Minimize : OptimizationType.Maximize);
    }
}