using System;
using System.IO;

namespace Sage
{
    class HashResult
    {
        readonly Action<TextWriter, TextWriter> _printValue;
        public string QueryName { get; }
        public int ExitCode { get; }
        HashResult(string queryName, Action<TextWriter, TextWriter> printValue, int exitCode)
        {
            QueryName = queryName;
            ExitCode = exitCode;
            _printValue = printValue;
        }

        public static HashResult Hash(string queryName, string hashValue) =>
            new HashResult(queryName, (stdOut, stdError) => stdOut.WriteLine(hashValue), exitCode: 0);

        public static HashResult Error(string queryName, string errorValue) =>
            new HashResult(queryName, (stdOut, stdError) => { stdOut.WriteLine(); stdError.WriteLine(errorValue); }, exitCode: 1);

        public void PrintValue(TextWriter stdOut, TextWriter stdError) =>
            _printValue(stdOut, stdError);
    }
}
