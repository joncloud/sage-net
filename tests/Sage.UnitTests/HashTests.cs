using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Sage.UnitTests
{
    public class HashTests
    {
        [Sql]
        public void JsonTests()
        {
            Tests(
                x => x.json(SqlAttribute.ConnectionString),
                "0x91738898BE6EFB426A36452F1542D8125D88BA477181C5050F18AA660B752A62",
                "0xD090F0B1DC045D93136B03DBE30DB9F3AB4777D12F512168549B191924C0EE2F"
            );
        }

        [Sql]
        public void TabTests()
        {
            Tests(
                x => x.tab(SqlAttribute.ConnectionString),
                "0x313EA196881D370AEEAF78E274B0D08541F6CBF0DDFC7BE57A4594AD0A752A5C",
                "0x54CB67D1746CD42CA947F6CE705060D0FB5540E55D588F5726CDAD0B73F41618"
            );
        }

        static void Tests(Action<Program> fn, string hash1, string hash2)
        {
            var queries = new[] {
                new Query
                {
                    Name = "Query1",
                    CommandText = "SELECT 1 [Num]"
                },
                new Query
                {
                    Name = "Query2",
                    CommandText = "SELECT 2 [Num]"
                }
            };
            var actual = CalculateHashes(queries, fn);
            var expected = new[]
            {
                new Hash("Query1", hash1),
                new Hash("Query2", hash2)
            };

            Assert.Equal(expected, actual);
        }

        static Hash CalculateHash(Query query, Action<Program> fn) =>
            Assert.Single(CalculateHashes(new[] { query }, fn));

        static IEnumerable<Hash> CalculateHashes(IEnumerable<Query> queries, Action<Program> fn)
        {
            using (Swap.ConsoleIn(WithJson(queries)))
            using (var writer = new StringWriter())
            using (Swap.ConsoleOut(writer))
            {
                fn(new Program());

                return writer.ToString()
                    .Split(Environment.NewLine)
                    .Where(IsNotEmpty)
                    .Select(AsHash)
                    .ToArray();
            }

            bool IsNotEmpty(string s) => !string.IsNullOrWhiteSpace(s);

            Hash AsHash(string s)
            {
                string[] parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return new Hash(parts[0], parts[1]);
            }

            TextReader WithJson(object o)
            {
                string json = JsonConvert.SerializeObject(o);
                return new StringReader(json);
            }
        }
    }
}
