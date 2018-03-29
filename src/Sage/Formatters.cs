using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;

namespace Sage
{
    class Formatters
    {
        public static void ReadDataTabDelimited(SqlCommand command, StreamWriter writer)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int i = reader.FieldCount;
                    while (--i >= 0)
                    {
                        writer.Write(reader[i]);
                        if (i > 0) writer.Write('\t');
                    }
                    writer.WriteLine();
                }
            }
        }

        public static void ReadDataAsJson(SqlCommand command, StreamWriter writer)
        {
            var jsonWriter = new JsonTextWriter(writer);
            using (var reader = command.ExecuteReader())
            {
                int fieldCount = reader.FieldCount;
                jsonWriter.WriteStartArray();
                while (reader.Read())
                {
                    jsonWriter.WriteWhitespace(Environment.NewLine);
                    jsonWriter.WriteWhitespace("  ");
                    jsonWriter.WriteStartObject();
                    jsonWriter.WriteWhitespace(Environment.NewLine);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        jsonWriter.WriteWhitespace("    ");
                        jsonWriter.WritePropertyName(reader.GetName(i));
                        jsonWriter.WriteValue(reader[i]);
                        jsonWriter.WriteWhitespace(Environment.NewLine);
                    }
                    jsonWriter.WriteWhitespace(Environment.NewLine);
                    jsonWriter.WriteWhitespace("  ");
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteWhitespace(Environment.NewLine);
                jsonWriter.WriteEndArray();
            }
            jsonWriter.Flush();
        }
    }
}
