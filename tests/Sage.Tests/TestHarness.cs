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
            using (var stdOutWriter = new StringWriter())
            using (Swap.ConsoleOut(stdOutWriter))
            using (var stdErrorWriter = new StringWriter())
            using (Swap.ConsoleError(stdErrorWriter))
            {
                int exitCode = fn();

                string stdOut = stdOutWriter.ToString();
                string stdError = stdErrorWriter.ToString();
                return (exitCode, stdOut, stdError);
            }

            TextReader WithJson(object o)
            {
                string json = JsonConvert.SerializeObject(o);
                return new StringReader(json);
            }
        }
    }
}
