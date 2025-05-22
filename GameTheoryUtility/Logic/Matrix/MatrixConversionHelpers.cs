using System.Buffers.Binary;
using System.Text.Json;

namespace GameTheoryUtility.Logic.Matrix;

public static class MatrixConversionHelpers
{
    public static void WriteMatrix(this Utf8JsonWriter writer, IMatrix matrix)
    {
        writer.WriteStartObject();
        writer.WriteNumber("rows", matrix.Size.Rows);
        writer.WriteNumber("columns", matrix.Size.Columns);
        writer.WriteStartArray("values");
        byte[] bytes = new byte[8];
        if (matrix is ReadonlyMatrix<int> integer)
            integer.Iterate((m, i, j) => writer.WriteNumberValue(m[i, j]));
        else if (matrix is ReadonlyMatrix<double> real)
            real.Iterate((m, i, j) =>
            {
                BinaryPrimitives.WriteDoubleBigEndian(bytes, m[i, j]);
                writer.WriteStringValue($"0x{Convert.ToHexString(bytes)}");
            });
        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static IMutableMatrix ReadMatrix(ref Utf8JsonReader reader)
    {
        int rows = -1;
        int columns = -1;
        bool isInteger = true;
        List<double> values = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                break;

            string propName = reader.GetString()!;
            reader.Read();

            switch (propName)
            {
                case "rows":
                    rows = reader.GetInt32();
                    break;
                case "columns":
                    columns = reader.GetInt32();
                    break;
                case "values":
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        if (reader.TokenType == JsonTokenType.Number)
                        {
                            if (isInteger && reader.TryGetInt32(out var integer))
                                values.Add(integer);
                            else
                            {
                                isInteger = false;
                                values.Add(reader.GetDouble());
                            }

                        }
                        else
                        {
                            isInteger = false;
                            values.Add(BinaryPrimitives.ReadDoubleBigEndian(Convert.FromHexString(reader.GetString()![2..])));
                        }
                    }
                    break;
                default:
                    continue;
            }
        }

        if (isInteger)
        {
            var result = new Matrix<int>(rows, columns);
            for (int i = 0, e = 0; i < rows; i++)
                for (int j = 0; j < columns; j++, e++)
                    result[i, j] = (int)values[e];
            return result;
        }
        else
        {
            var result = new Matrix<double>(rows, columns);
            for (int i = 0, e = 0; i < rows; i++)
                for (int j = 0; j < columns; j++, e++)
                    result[i, j] = values[e];
            return result;

        }
    }
}
