using System;
using System.IO;

namespace Sage.UnitTests
{
    static class Ignore
    {
        public static IDisposable ConsoleIn() =>
            Swap.ConsoleIn(TextReader.Null);
        public static IDisposable ConsoleOut() =>
            Swap.ConsoleOut(TextWriter.Null);
        public static IDisposable ConsoleError() =>
            Swap.ConsoleError(TextWriter.Null);
    }
}
