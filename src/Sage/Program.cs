using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ThorNet;

namespace Sage
{
    [Option("out-dir", "-o", "writes the data out into a directory")]
    [Option("hash", "-h", "chooses the hash algorithm to use", DefaultValue = "SHA256")]
    partial class Program : Thor
    {
        static int Main(string[] args) => Start<Program>(args);

        [Desc("tab CONNECTION-STRING", "displays hashes for queries defined in stdin")]
        public void tab(string connectionString)
        {
            using (var outDir = GetOutDir())
            using (var algorithm = GetHashAlgorithm())
            {
                var queries = ReadQueriesFromStdIn();
                DisplayHashesFor(connectionString, queries, outDir, Formatters.ReadDataTabDelimited, algorithm);
            }
        }

        [Desc("json CONNECTION-STRING", "displays hashes for queries defined in stdin")]
        public void json(string connectionString)
        {
            using (var outDir = GetOutDir())
            using (var algorithm = GetHashAlgorithm())
            {
                var queries = ReadQueriesFromStdIn();
                DisplayHashesFor(connectionString, queries, outDir, Formatters.ReadDataAsJson, algorithm);
            }
        }

        OutDir GetOutDir() =>
            Option("out-dir", x => new OutDir(x), () => new OutDir());

        HashAlgorithm GetHashAlgorithm() =>
            Option("hash", x => Hashes.Create(x));

        static void DisplayHashesFor(string connectionString, List<Query> queries, OutDir outDir, Action<SqlCommand, StreamWriter> fn, HashAlgorithm algorithm)
        {
            var padding = queries.Select(q => q.Name.Length).Max();

            foreach (var query in queries)
            {
                DisplayHashFor(connectionString, query.Name.PadRight(padding, ' '), query.CommandText, outDir, fn, algorithm);
            }
        }

        static List<Query> ReadQueriesFromStdIn()
        {
            using (var stream = Console.OpenStandardInput())
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<List<Query>>(jsonReader);
            }
        }

        static void DisplayHashFor(string connectionString, string name, string commandText, OutDir outDir, Action<SqlCommand, StreamWriter> fn, HashAlgorithm algorithm)
        {
            Console.Write(name);
            Console.Write("\t");
            string hash = ComputeHash(connectionString, name, commandText, outDir, fn, algorithm);
            Console.WriteLine(hash);
        }

        static string ComputeHash(string connectionString, string name, string commandText, OutDir outDir, Action<SqlCommand, StreamWriter> fn, HashAlgorithm algorithm)
        {
            string file = outDir.AddFile(name);
            string text;
            using (var stream = LoadData(connectionString, file, commandText, fn))
            {
                byte[] hash = algorithm.ComputeHash(stream);
                StringBuilder sb = new StringBuilder(2 + hash.Length * 2);

                sb.Append("0x");
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                text = sb.ToString();
            }
            return text;
        }

        static Stream LoadData(string connectionString, string file, string commandText, Action<SqlCommand, StreamWriter> fn)
        {
            var stream = File.Create(file);
            var writer = new StreamWriter(stream, Encoding.UTF8, 81920, leaveOpen: true)
            {
                AutoFlush = true
            };

            using (var connection = new SqlConnection(connectionString))
            using (var command = connection.CreateCommand())
            {
                command.CommandTimeout = 240;
                command.CommandText = commandText;

                connection.Open();
                fn(command, writer);
                connection.Close();
            }

            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
