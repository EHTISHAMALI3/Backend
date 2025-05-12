using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hidayah.Application.Services.Decryption
{
    public class DecryptionService
    {
        private readonly string _privateKey;

        public DecryptionService(string privateKey)
        {
            _privateKey = privateKey;
        }

        public string DecryptWithPrivateKey(string encryptedBase64)
        {
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
                // Load the private key from PEM string
                AsymmetricCipherKeyPair keyPair;
                using (var stringReader = new StringReader(_privateKey))
                {
                    var pemReader = new PemReader(stringReader);
                    keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
                }

                // Create RSA engine
                var rsaEngine = new Pkcs1Encoding(new RsaEngine());
                rsaEngine.Init(false, keyPair.Private);

                // Decrypt the data
                byte[] decryptedBytes = rsaEngine.ProcessBlock(encryptedBytes, 0, encryptedBytes.Length);

                // Convert decrypted bytes to string
                return Encoding.UTF8.GetString(decryptedBytes);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption failed: {ex.Message}");
                return null;  // Or handle the error in a way that doesn’t interrupt Swagger or the main app.
            }
        }
    }
}