using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sage.Tests
{
    static class TestHarness
    {
        public static (int, string, string) Run(Func<int> fn, string commandText)
        {
            return Run(fn, new StringReader($"\"{commandText}\"\x1a"));
        }

        public static (int, string, string) Run(Func<int> fn, IEnumerable<Query> queries)
        {
            return Run(fn, WithJson(queries));

            TextReader WithJson(object o)
            {
                string json = JsonConvert.SerializeObject(o);
                return new StringReader(json);
            }
        }

        static (int, string, string) Run(Func<int> fn, TextReader stdIn)
        {
            using (Swap.ConsoleIn(stdIn))
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
        }
    }
}
