using System;
using System.Security.Cryptography;

namespace Sage
{
    class Hashes
    {
        public const string DefaultAlgorithmName = nameof(HashAlgorithmName.SHA256);
        public static HashAlgorithm CreateDefault() =>
            Create(DefaultAlgorithmName);
        public static HashAlgorithm Create(string name)
        {
            switch (name?.ToUpper())
            {
                case nameof(HashAlgorithmName.MD5):
                    return MD5.Create();
                case nameof(HashAlgorithmName.SHA1):
                    return SHA1.Create();
                case nameof(HashAlgorithmName.SHA256):
                    return SHA256.Create();
                case nameof(HashAlgorithmName.SHA384):
                    return SHA384.Create();
                case nameof(HashAlgorithmName.SHA512):
                    return SHA512.Create();
                default: throw new ArgumentOutOfRangeException(nameof(name));
            }
        }
    }
}
