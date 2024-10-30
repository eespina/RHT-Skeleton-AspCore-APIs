using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Extensions
{
    public interface ICredentialHandler
    {
        Task<string> DecryptStringAES(string cipherText, string encryptionDecryptionKey);
        Task<string> EncryptStringAES(string cipherText, string encryptionDecryptionKey);
    }

    public class CredentialHandler : ICredentialHandler
    {
        private readonly ILogger<CredentialHandler> logger;

        public CredentialHandler(ILogger<CredentialHandler> logger)
        {
            this.logger = logger;
        }

        public async Task<string> DecryptStringAES(string cipherText, string encryptionDecryptionKey)
        {
            try
            {
                logger.LogInformation("INSIDE " + MethodBase.GetCurrentMethod());
                var keybytes = Encoding.UTF8.GetBytes(encryptionDecryptionKey);
                var iv = Encoding.UTF8.GetBytes(encryptionDecryptionKey);

                var encrypted = Convert.FromBase64String(cipherText);
                var decriptedFromJavascript = await DecryptStringFromBytes(encrypted, keybytes, iv);
                return decriptedFromJavascript;

            }
            catch (Exception ex)
            {
                var t1 = ex.ToString(); //TODO - LOG Error
                throw;
            }
        }

        private async Task<string> DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream
                                // and place them in a string.
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch (Exception exception)
                {
                    plaintext = exception.ToString();
                }
            }

            return plaintext;
        }

        public async Task<string> EncryptStringAES(string plainText, string encryptionDecryptionKey)
        {
            logger.LogInformation("INSIDE " + MethodBase.GetCurrentMethod());
            var keybytes = Encoding.UTF8.GetBytes(encryptionDecryptionKey);
            var iv = Encoding.UTF8.GetBytes(encryptionDecryptionKey);

            var encryoFromJavascript = await EncryptStringToBytes(plainText, keybytes, iv);
            return Convert.ToBase64String(encryoFromJavascript);
        }

        private async Task<byte[]> EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object
            // with the specified key and IV.
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
    }
}
