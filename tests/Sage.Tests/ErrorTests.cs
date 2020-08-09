using System.Collections.Generic;
using Xunit;

namespace Sage.Tests
{
    public class ErrorTests
    {
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [SqlTheory]
        public void ShouldSetExitCodeOneGivenAnyError(int insertPosition)
        {
            var queries = new List<Query>
            {
                new Query { Name = "1", CommandText = "SELECT 1" },
                new Query { Name = "2", CommandText = "SELECT 2" }
            };

            queries.Insert(insertPosition, new Query
            {
                Name = "3",
                CommandText = "SELECT X"
            });

            var (exitCode, _, __) = TestHarness.Run(() => Program.AsTabbed(Sql.ConnectionString), queries);
            Assert.Equal(1, exitCode);
        }

        [SqlFact]
        public void ShouldWriteErrorsToStdErrorGivenError()
        {
            var queries = new []
            {
                new Query { Name = "1" , CommandText = "SELECT x" }
            };
            var (_, __, stdError) = TestHarness.Run(() => Program.AsTabbed("abc"), queries);
            Assert.NotEmpty(stdError);
        }
    }
}