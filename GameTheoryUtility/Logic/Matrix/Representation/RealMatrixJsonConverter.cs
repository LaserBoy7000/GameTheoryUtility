using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Matrix.Representation;

public class RealMatrixJsonConverter : JsonConverter<Matrix<double>>
{
    public override Matrix<double>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        MatrixConversionHelpers.ReadMatrix(ref reader).ToReal();

    public override void Write(Utf8JsonWriter writer, Matrix<double> value, JsonSerializerOptions options) => writer.WriteMatrix(value);
}
