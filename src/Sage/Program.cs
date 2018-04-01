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
    [Option("hash", "-h", "chooses the hash algorithm to use", DefaultValue = Hashes.DefaultAlgorithmName)]
    public class Program : Thor
    {
        static int Main(string[] args) => Start<Program>(args);

        protected override string GetPackageName() => "sage";

        [Desc("tab CONNECTION-STRING", "queries the sql server through the connection string and displays hashes in tab-delimited format")]
        [LongDesc(@"
sage tab $CONNECTION_STRING will prompt for queries to run through stdin.

Queries are defined with the following format:
[
  {
    ""name"": ""Query1"",
    ""commandText"": ""SELECT 1 [Num]"",
  },
  {
    ""name"": ""Query2"",
    ""commandText"": ""SELECT 2 [Num]"",
  }
]

`$QUERIES | sage tab $CONNECTION_STRING` will result in 
Query1  0x313EA196881D370AEEAF78E274B0D08541F6CBF0DDFC7BE57A4594AD0A752A5C
Query2  0x54CB67D1746CD42CA947F6CE705060D0FB5540E55D588F5726CDAD0B73F41618
")]
        public void tab(string connectionString)
        {
            using (var outDir = GetOutDir())
            using (var algorithm = GetHashAlgorithm())
            {
                var queries = ReadQueriesFromStdIn();
                DisplayHashesFor(connectionString, queries, outDir, Formatters.ReadDataTabDelimited, algorithm);
            }
        }

        public static int AsTabbed(string connectionString, string outDir = "", string hash = "")
        {
            List<string> args = new List<string>
            {
                nameof(tab),
                connectionString
            };

            if (!string.IsNullOrWhiteSpace(outDir)) args.Add($"--out-dir={outDir}");
            if (!string.IsNullOrWhiteSpace(hash)) args.Add($"--hash={hash}");

            return Main(args.ToArray());
        }

        [Desc("json CONNECTION-STRING", "queries the sql server through the connection string and displays hashes in json format")]
        [LongDesc(@"
sage json $CONNECTION_STRING will prompt for queries to run through stdin.

Queries are defined with the following format:
[
  {
    ""name"": ""Query1"",
    ""commandText"": ""SELECT 1 [Num]"",
  },
  {
    ""name"": ""Query2"",
    ""commandText"": ""SELECT 2 [Num]"",
  }
]

`$QUERIES | sage tab $CONNECTION_STRING` will result in 
Query1  0x91738898BE6EFB426A36452F1542D8125D88BA477181C5050F18AA660B752A62
Query2  0xD090F0B1DC045D93136B03DBE30DB9F3AB4777D12F512168549B191924C0EE2F
")]
        public void json(string connectionString)
        {
            using (var outDir = GetOutDir())
            using (var algorithm = GetHashAlgorithm())
            {
                var queries = ReadQueriesFromStdIn();
                DisplayHashesFor(connectionString, queries, outDir, Formatters.ReadDataAsJson, algorithm);
            }
        }

        public static int AsJson(string connectionString, string outDir = "", string hash = "")
        {
            List<string> args = new List<string>
            {
                nameof(json),
                connectionString
            };

            if (!string.IsNullOrWhiteSpace(outDir)) args.Add($"--out-dir={outDir}");
            if (!string.IsNullOrWhiteSpace(hash)) args.Add($"--hash={hash}");

            return Main(args.ToArray());
        }

        OutDir GetOutDir() =>
            Option("out-dir", x => new OutDir(x), () => new OutDir());

        HashAlgorithm GetHashAlgorithm() =>
            Option("hash", x => Hashes.Create(x), Hashes.CreateDefault);

        static void DisplayHashesFor(string connectionString, List<Query> queries, OutDir outDir, Formatter fn, HashAlgorithm algorithm)
        {
            var padding = queries.Select(q => q.Name.Length).Max();

            foreach (var query in queries)
            {
                DisplayHashFor(connectionString, query.Name.PadRight(padding, ' '), query.CommandText, outDir, fn, algorithm);
            }
        }

        static List<Query> ReadQueriesFromStdIn()
        {
            using (var jsonReader = new JsonTextReader(Console.In))
            {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<List<Query>>(jsonReader);
            }
        }

        static void DisplayHashFor(string connectionString, string name, string commandText, OutDir outDir, Formatter fn, HashAlgorithm algorithm)
        {
            Console.Out.Write(name);
            Console.Out.Write("  ");
            string hash = ComputeHash(connectionString, name, commandText, outDir, fn, algorithm);
            Console.Out.WriteLine(hash);
        }

        static string ComputeHash(string connectionString, string name, string commandText, OutDir outDir, Formatter fn, HashAlgorithm algorithm)
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

        static Stream LoadData(string connectionString, string file, string commandText, Formatter fn)
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
