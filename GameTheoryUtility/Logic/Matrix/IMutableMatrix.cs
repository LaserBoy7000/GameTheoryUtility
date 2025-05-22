using GameTheoryUtility.Logic.Matrix.Representation;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Matrix;

[JsonConverter(typeof(MutableMatrixJsonConverter))]
public interface IMutableMatrix : IMatrix
{
    public IMatrix MakeReadonly();
}
