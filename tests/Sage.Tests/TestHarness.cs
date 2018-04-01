using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sage.Tests
{
    static class TestHarness
    {
        public static (int, string, string) Run(Action fn, IEnumerable<Query> queries)
        {
            using (Swap.ConsoleIn(WithJson(queries)))
            using (var writer = new StringWriter())
            using (Swap.ConsoleOut(writer))
            {
                fn();

                string stdOut = writer.ToString();
                return (0, stdOut, "");
            }

            TextReader WithJson(object o)
            {
                string json = JsonConvert.SerializeObject(o);
                return new StringReader(json);
            }
        }
    }
}
