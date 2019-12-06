using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Sage.Tests
{
    public class SimpleTests
    {
        [SqlFact]
        public void ShouldHandleSimpleCommandText()
        {
            var commandText = "SELECT 1 [Num]";
            var (exitCode, stdOut, stdError) = TestHarness.Run(
                () => Program.AsTabbed(Sql.ConnectionString),
                commandText
            );

            Assert.Equal(0, exitCode);
            Assert.Empty(stdError);
            var hashes = stdOut.Split(Environment.NewLine)
                .Where(IsNotEmpty)
                .Select(AsHash)
                .ToArray();

            var actual = Assert.Single(hashes);
            var expected = new Hash(
                "Query",
                "0x313EA196881D370AEEAF78E274B0D08541F6CBF0DDFC7BE57A4594AD0A752A5C"
            );

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
