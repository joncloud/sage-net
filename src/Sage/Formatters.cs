using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;

namespace Sage
{
    delegate void Formatter(SqlCommand command, StreamWriter writer);

    class Formatters
    {
        static readonly string _newLine = "\n";

        public static void ReadDataTabDelimited(SqlCommand command, StreamWriter writer)
        {
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int count = reader.FieldCount;
                    for (int i = 0; i < count; i++)
                    {
                        writer.Write(reader[i]);
                        if (i < count - 1) writer.Write('\t');
                    }
                    writer.Write(_newLine);
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
                    jsonWriter.WriteWhitespace(_newLine);
                    jsonWriter.WriteWhitespace("  ");
                    jsonWriter.WriteStartObject();
                    jsonWriter.WriteWhitespace(_newLine);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        jsonWriter.WriteWhitespace("    ");
                        jsonWriter.WritePropertyName(reader.GetName(i));
                        jsonWriter.WriteValue(reader[i]);
                        jsonWriter.WriteWhitespace(_newLine);
                    }
                    jsonWriter.WriteWhitespace(_newLine);
                    jsonWriter.WriteWhitespace("  ");
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteWhitespace(_newLine);
                jsonWriter.WriteEndArray();
            }
            jsonWriter.Flush();
        }
    }
}
