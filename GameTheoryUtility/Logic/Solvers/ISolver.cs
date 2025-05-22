using GameTheoryUtility.Controls;
using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Matrix;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Solvers;

[JsonDerivedType(typeof(CleanStrategySolver), "clean-strategies")]
[JsonDerivedType(typeof(LinearProgrammingSolver), "linear-optimization")]
[JsonDerivedType(typeof(BrownRobinsonSolver), "brown-robinson")]
[JsonDerivedType(typeof(WaldCriterionSolver), "wald")]
[JsonDerivedType(typeof(SavageCriterionSolver), "savage")]
[JsonDerivedType(typeof(LagrangeCriterionSolver), "lagrange")]
[JsonDerivedType(typeof(HurwitzCriterionSolver), "hurwitz")]
public interface ISolver
{
    public int LogicalPriority { get; }
    public string Name { get; }
    public SolverViewModel CreateViewModel(bool initialIsEnabled = false);
    public void Solve(GameComputationInstance instance);
    public bool IsApplicable(IMatrixGameParameters parameters) => true;
    public ISolver Clone();
}
