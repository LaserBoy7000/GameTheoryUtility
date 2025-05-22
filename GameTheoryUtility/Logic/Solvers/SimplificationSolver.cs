using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Computations;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Solvers;

public class SimplificationSolver : ISolver
{
    [JsonIgnore]
    public int LogicalPriority => 0;

    public string Name => "Попередня нормалізація матриці";

    public ISolver Clone() => new SimplificationSolver();

    public SolverViewModel CreateViewModel(bool initialIsEnabled = false)
    {
        throw new NotImplementedException();
    }

    public void Solve(GameComputationInstance instance)
    {
        throw new NotImplementedException();
    }
}
