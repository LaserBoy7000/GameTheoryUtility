using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Matrix.Representation;

public class IntegerMatrixJsonConverter : JsonConverter<Matrix<int>>
{
    public override Matrix<int>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        MatrixConversionHelpers.ReadMatrix(ref reader).Round();

    public override void Write(Utf8JsonWriter writer, Matrix<int> value, JsonSerializerOptions options) => writer.WriteMatrix(value);
}
