using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Matrix;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Solvers;

public class LagrangeCriterionSolver : ISolver
{
    [JsonIgnore]
    public int LogicalPriority => 1;
    [JsonIgnore]
    public string Name => "Критерій Kагранжа";

    public void Solve(GameComputationInstance instance)
    {

    }

    public bool IsApplicable(IMatrixGameParameters parameters) => parameters.IsPassiveSecondPlayer;

    public SolverViewModel CreateViewModel(bool initialIsEnabled = false) => new SolverViewModel(this, initialIsEnabled);

    public ISolver Clone() => new LagrangeCriterionSolver();
}
