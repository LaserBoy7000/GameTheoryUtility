using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Matrix.Representation;

public class MatrixJsonConverter : JsonConverter<IMatrix>
{
    public override IMatrix? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
         MatrixConversionHelpers.ReadMatrix(ref reader);

    public override void Write(Utf8JsonWriter writer, IMatrix value, JsonSerializerOptions options) => writer.WriteMatrix(value);
}
