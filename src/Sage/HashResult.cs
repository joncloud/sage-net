namespace Sage
{
    class HashResult
    {
        public string QueryName { get; }
        public string HashOrErrorValue { get; }
        HashResult(string queryName, string hashOrErrorValue)
        {
            QueryName = queryName;
            HashOrErrorValue = hashOrErrorValue;
        }

        public static HashResult Hash(string queryName, string hashValue) =>
            new HashResult(queryName, hashValue);

        public static HashResult Error(string queryName, string errorValue) =>
            new HashResult(queryName, errorValue);
    }
}
