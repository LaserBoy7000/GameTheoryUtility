using GameTheoryUtility.Logic.Matrix;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Matrix.Representation;

public class MutableMatrixJsonConverter : JsonConverter<IMutableMatrix>
{
    public override IMutableMatrix? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
         MatrixConversionHelpers.ReadMatrix(ref reader);

    public override void Write(Utf8JsonWriter writer, IMutableMatrix value, JsonSerializerOptions options) => writer.WriteMatrix(value);
}
