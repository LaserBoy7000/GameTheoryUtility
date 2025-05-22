using GameTheoryUtility.Logic.Computations;
using GameTheoryUtility.Logic.Matrix;
using GameTheoryUtility.Logic.Solvers;

namespace GameTheoryUtility.Logic.Game;

public class GameEnvironment
{
    public GameEnvironment()
    {
        Parameters = new();
        Matrix = Parameters.CreateMutableGameMatrix();
    }

    public GeneratedMatrixGameParameters Parameters { get; set; }
    public IMutableMatrix Matrix { get; set; }
    public List<ISolver> Solvers { get; } = [];
    public GameComputationInstance? LastComputation { get; set; } = null;

    public IMutableMatrix GenerateMatrix() =>
        Matrix = Parameters.GenerateMutableGameMatrix();

    public IMutableMatrix UpdateMatrix()
    {
        var mx = Parameters.CreateMutableGameMatrix();
        mx.MigrateValues(Matrix);
        Matrix = mx;
        return mx;
    }

    public GameEnvironment Clone()
    {
        GameEnvironment clone = new GameEnvironment();
        clone.Matrix = Matrix.Clone();
        clone.Parameters = Parameters.Clone();
        clone.Parameters.GeneratorSeed = null;
        foreach(var s in Solvers)
            clone.Solvers.Add(s.Clone());
        return clone;
    }

    public GameComputationInstance CreateNewComputation(bool isIndependant = false) => new GameComputationInstance(isIndependant ? Parameters.Clone() : Parameters, Matrix.MakeReadonly(), Solvers.OrderBy(x => x.LogicalPriority));
}
