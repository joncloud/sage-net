namespace Sage
{
    class HashResult
    {
        public string QueryName { get; }
        public string HashOrErrorValue { get; }
        public int ExitCode { get; }
        HashResult(string queryName, string hashOrErrorValue, int exitCode)
        {
            QueryName = queryName;
            HashOrErrorValue = hashOrErrorValue;
            ExitCode = exitCode;
        }

        public static HashResult Hash(string queryName, string hashValue) =>
            new HashResult(queryName, hashValue, exitCode: 0);

        public static HashResult Error(string queryName, string errorValue) =>
            new HashResult(queryName, errorValue, exitCode: 1);
    }
}
