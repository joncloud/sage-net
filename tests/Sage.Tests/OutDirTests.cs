using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sage.Tests
{
    public class OutDirTests
    {
        [SqlFact]
        public void ShouldPersistFilesGivenOutDir()
        {
            using (var testDirectory = new TestDirectory())
            {
                var _ = TestHarness.Run(
                    () => Program.AsJson(Sql.ConnectionString, testDirectory.AbsolutePath),
                    new[]
                    {
                        new Query { Name = "Query1", CommandText = "SELECT 1 [Num]" },
                        new Query { Name = "Query2", CommandText = "SELECT 2 [Num]" },
                        new Query { Name = "Query3", CommandText = "SELECT 3 [Num]" },
                        new Query { Name = "Query4", CommandText = "SELECT 4 [Num]" }
                    }
                );

                var actual = testDirectory.GetFiles().Select(x => x.Name).OrderBy(x => x);
                var expected = new[] { "Query1", "Query2", "Query3", "Query4" };
                Assert.Equal(expected, actual);
            }
        }

        [SqlFact]
        public void Json_ShouldSerializeJsonArrayToOutDir()
        {
            Test(
                path => Program.AsJson(Sql.ConnectionString, path),
                file => JsonConvert.DeserializeObject<Mock[]>(File.ReadAllText(file.FullName))
            );
        }

        [SqlFact]
        public void Tab_ShouldSerializeTabDelimitedLinesToOutDir()
        {
            Test(
                path => Program.AsTabbed(Sql.ConnectionString, path),
                file => File.ReadAllLines(file.FullName).Select(AsMock)
            );

            Mock AsMock(string s) {
                string[] parts = s.Split('\t');
                return new Mock {
                    Number = int.Parse(parts[0]),
                    Text = parts[1]
                };
            }
        }

        static void Test(Action<string> fn, Func<FileInfo, IEnumerable<Mock>> convert)
        {
            using (var testDirectory = new TestDirectory())
            {
                var _ = TestHarness.Run(
                    () => fn(testDirectory.AbsolutePath),
                    new[] {
                        new Query { 
                            Name = "data", 
                            CommandText = @"
                                SELECT 123 [Number], 'ABC' [Text]
                                UNION ALL SELECT 456 [Number], 'DEF' [Text]
                            "
                        }
                    }
                );

                var file = Assert.Single(testDirectory.GetFiles());

                var actual = convert(file);
                var expected = new[] {
                    new Mock { Number = 123, Text = "ABC" },
                    new Mock { Number = 456, Text = "DEF" }
                };
                Assert.Equal(expected, actual);
            }
        }

        struct Mock
        {
            public int Number { get; set; }
            public string Text { get; set; }

            public override string ToString() =>
                $"Number ({Number}), Text ({Text})";
        }

        class TestDirectory : IDisposable
        {
            readonly DirectoryInfo _directory;
            public string AbsolutePath => _directory.FullName;

            public TestDirectory()
            {
                string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                _directory = Directory.CreateDirectory(path);
            }

            public IEnumerable<FileInfo> GetFiles() =>
                _directory.GetFiles();

            public void Dispose()
            {
                _directory.Delete(recursive: true);
            }
        }
    }
}
