using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace Monizze.Common.Extensions
{
    public static class Extensions
    {
        public static string ToMd5Hash(this IBuffer value)
        {
            if (value == null)
                return string.Empty;
            var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var hashed = hasher.HashData(value);
            return CryptographicBuffer.EncodeToHexString(hashed);
        }
    }
}
