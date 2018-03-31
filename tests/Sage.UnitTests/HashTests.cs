using System;
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
                () => Program.AsJson(SqlAttribute.ConnectionString),
                "0x91738898BE6EFB426A36452F1542D8125D88BA477181C5050F18AA660B752A62",
                "0xD090F0B1DC045D93136B03DBE30DB9F3AB4777D12F512168549B191924C0EE2F"
            );
        }

        [Sql]
        public void TabTests()
        {
            Tests(
                () => Program.AsTabbed(SqlAttribute.ConnectionString),
                "0x313EA196881D370AEEAF78E274B0D08541F6CBF0DDFC7BE57A4594AD0A752A5C",
                "0x54CB67D1746CD42CA947F6CE705060D0FB5540E55D588F5726CDAD0B73F41618"
            );
        }

        static void Tests(Action fn, string hash1, string hash2)
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
            var (_, stdOut, __) = TestHarness.Run(fn, queries);
            var actual = stdOut.Split(Environment.NewLine)
                .Where(IsNotEmpty)
                .Select(AsHash)
                .ToArray();
            var expected = new[]
            {
                new Hash("Query1", hash1),
                new Hash("Query2", hash2)
            };

            Assert.Equal(expected, actual);

            bool IsNotEmpty(string s) => !string.IsNullOrWhiteSpace(s);

            Hash AsHash(string s)
            {
                string[] parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                return new Hash(parts[0], parts[1]);
            }
        }
    }
}
