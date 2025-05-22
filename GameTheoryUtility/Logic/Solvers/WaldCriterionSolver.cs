using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Matrix;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Solvers;

public class WaldCriterionSolver : ISolver
{
    [JsonIgnore]
    public int LogicalPriority => 1;
    [JsonIgnore]
    public string Name => "Критерій Вальда";

    public void Solve(GameComputationInstance instance)
    {
    }

    public SolverViewModel CreateViewModel(bool initialIsEnabled = false) => new SolverViewModel(this, initialIsEnabled);

    public bool IsApplicable(IMatrixGameParameters parameters) => parameters.IsPassiveSecondPlayer;

    public ISolver Clone() => new WaldCriterionSolver();
}
