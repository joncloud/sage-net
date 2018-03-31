using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Sage.UnitTests
{
    public class OutDirTests
    {
        [Sql]
        public void ShouldPersistFilesGivenOutDir()
        {
            using (var testDirectory = new TestDirectory())
            {
                var _ = TestHarness.Run(
                    () => Program.AsJson(SqlAttribute.ConnectionString, testDirectory.AbsolutePath),
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

        [Sql]
        public void Json_ShouldSerializeJsonArrayToOutDir()
        {
            using (var testDirectory = new TestDirectory())
            {
                var _ = TestHarness.Run(
                    () => Program.AsJson(SqlAttribute.ConnectionString, testDirectory.AbsolutePath),
                    new[] {
                        new Query { 
                            Name = "Json", 
                            CommandText = @"
                                SELECT 123 [Number], 'ABC' [Text]
                                UNION ALL SELECT 456 [Number], 'DEF' [Text]
                            "
                        }
                    }
                );

                var file = Assert.Single(testDirectory.GetFiles());
                var json = File.ReadAllText(file.FullName);
                
                var actual = JsonConvert.DeserializeObject<Mock[]>(json);
                var expected = new[] {
                    new Mock { Number = 123, Text = "ABC" },
                    new Mock { Number = 456, Text = "DEF" }
                };
                Assert.Equal(expected, actual);
            }
        }

        [Sql]
        public void Tab_ShouldSerializeTabDelimitedLinesToOutDir()
        {
            using (var testDirectory = new TestDirectory())
            {
                var _ = TestHarness.Run(
                    () => Program.AsTabbed(SqlAttribute.ConnectionString, testDirectory.AbsolutePath),
                    new[] {
                        new Query { 
                            Name = "Tabbed", 
                            CommandText = @"
                                SELECT 123 [Number], 'ABC' [Text]
                                UNION ALL SELECT 456 [Number], 'DEF' [Text]
                            "
                        }
                    }
                );

                var file = Assert.Single(testDirectory.GetFiles());

                var actual = File.ReadAllLines(file.FullName)
                    .Select(AsMock);
                var expected = new[] {
                    new Mock { Number = 123, Text = "ABC" },
                    new Mock { Number = 456, Text = "DEF" }
                };
                Assert.Equal(expected, actual);

                Mock AsMock(string s) {
                    string[] parts = s.Split('\t');
                    return new Mock {
                        Number = int.Parse(parts[0]),
                        Text = parts[1]
                    };
                }
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
