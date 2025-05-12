using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.OpenSsl;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Hidayah.Application.Services.Encryption
{
    public class EncryptionService
    {
        private readonly string _publicKey;
        
        public EncryptionService(string publicKey)
        {
            _publicKey = publicKey;
        }
        public string EncryptWithPublicKey(string plainText)
        {
            try
            {
                // Convert plaintext to byte array
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                // Load the public key from PEM string
                AsymmetricKeyParameter publicKeyParam;
                using (var stringReader = new StringReader(_publicKey))
                {
                    var pemReader = new PemReader(stringReader);
                    publicKeyParam = (AsymmetricKeyParameter)pemReader.ReadObject();
                }

                // Create RSA engine with PKCS1 v1.5 padding
                var rsaEngine = new Pkcs1Encoding(new RsaEngine());
                rsaEngine.Init(true, publicKeyParam);

                // Encrypt the data
                byte[] encryptedBytes = rsaEngine.ProcessBlock(plainBytes, 0, plainBytes.Length);

                // Convert to base64 for transport
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption failed: {ex.Message}");
                return null;
            }
        }
    }
}
