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
                "0x36E549D78B1D6F651B88CC73659EE3BF59C9C6489034C3133AE88EFD605EF4B4",
                "0xCCD7EDB7DEEB767B6B2FF674C36F6A87A9A58C826730FDCA42A3D63EB744872A"
            );
        }

        [Sql]
        public void TabTests()
        {
            Tests(
                x => x.tab(SqlAttribute.ConnectionString),
                "0x07846B783C23904A75834C1932BC2FAD41E388E18BC32FFD73ED87BFA3844D87",
                "0x24C813CB6F9BADE379034E39A4665BF3BE056293A53543502481E4F3DB7E9291"
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
