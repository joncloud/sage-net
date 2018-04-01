using System;
using System.IO;

namespace Sage.Tests
{
    class Swap : IDisposable
    {
        readonly Action _back;
        public Swap(Action swap, Action back)
        {
            swap();
            _back = back;
        }

        public void Dispose() => _back();

        public static IDisposable ConsoleIn(TextReader reader)
        {
            TextReader original = Console.In;
            return new Swap(() => Console.SetIn(reader), () => Console.SetIn(original));
        }

        public static IDisposable ConsoleError(TextWriter writer)
        {
            TextWriter original = Console.Error;
            return new Swap(() => Console.SetError(writer), () => Console.SetError(original));
        }

        public static IDisposable ConsoleOut(TextWriter writer)
        {
            TextWriter original = Console.Out;
            return new Swap(() => Console.SetOut(writer), () => Console.SetOut(original));
        }
    }
}
