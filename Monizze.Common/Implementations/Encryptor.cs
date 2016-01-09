using System;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;
using Monizze.Common.Interfaces;

namespace Monizze.Common.Implementations
{
    public class Encryptor: IEncryptor
    {
        private const BinaryStringEncoding Encoding = BinaryStringEncoding.Utf8;
        private const string StringDescriptor = "LOCAL=user";

        public async Task<IBuffer> ProtectAsync(string strMsg)
        {
            // Create a DataProtectionProvider object for the specified descriptor.
            var provider = new DataProtectionProvider(StringDescriptor);
            // Encode the plaintext input message to a buffer.
            var buffMsg = CryptographicBuffer.ConvertStringToBinary(strMsg, Encoding);
            // Encrypt the message.
            var buffProtected = await provider.ProtectAsync(buffMsg);
            // Execution of the SampleProtectAsync function resumes here
            // after the awaited task (Provider.ProtectAsync) completes.
            return buffProtected;
        }

        public async Task<string> UnprotectAsync(IBuffer buffProtected)
        {
            // Create a DataProtectionProvider object.
            var provider = new DataProtectionProvider();
            // Decrypt the protected message specified on input.
            var buffUnprotected = await provider.UnprotectAsync(buffProtected);
            // Execution of the SampleUnprotectData method resumes here
            // after the awaited task (Provider.UnprotectAsync) completes
            // Convert the unprotected message from an IBuffer object to a string.
            var strClearText = CryptographicBuffer.ConvertBinaryToString(Encoding, buffUnprotected);
            // Return the plaintext string.
            return strClearText;
        }
    }
}
