using GameTheoryUtility.Logic.Matrix.Representation;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Matrix;

[JsonConverter(typeof(MatrixJsonConverter))]
public interface IMatrix
{
    public MatrixSize Size { get; }
}
