using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sage.Tests
{
    static class TestHarness
    {
        public static (int, string, string) Run(Func<int> fn, IEnumerable<Query> queries)
        {
            using (Swap.ConsoleIn(WithJson(queries)))
            using (var writer = new StringWriter())
            using (Swap.ConsoleOut(writer))
            {
                int exitCode = fn();

                string stdOut = writer.ToString();
                return (exitCode, stdOut, "");
            }

            TextReader WithJson(object o)
            {
                string json = JsonConvert.SerializeObject(o);
                return new StringReader(json);
            }
        }
    }
}
