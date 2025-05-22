using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Matrix;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Solvers;

public class BrownRobinsonSolver : ISolver
{
    [JsonIgnore]
    public int LogicalPriority => 2;
    [JsonIgnore]
    public string Name => "Метод Брауна-Робінсона";

    public ISolver Clone() => new BrownRobinsonSolver();

    public SolverViewModel CreateViewModel(bool initialIsEnabled = false) => new SolverViewModel(this, initialIsEnabled);

    public bool IsApplicable(IMatrixGameParameters parameters) => !parameters.IsPassiveSecondPlayer;

    public void Solve(GameComputationInstance instance)
    {
    }
}
