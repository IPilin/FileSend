using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace FileSend.Classes
{
    public class CryptoClass : IDisposable
    {
        Aes Aes;

        RSACryptoServiceProvider Rsa;

        /// <summary>
        /// Using Aes256, Rsa key length is 2048 bits. 
        /// Generate new keys for Aes and Rsa
        /// </summary>
        public CryptoClass()
        {
            Aes = Aes.Create();
            Aes.KeySize = 256;

            GenerateKeyAes();
            GenerateKeyRsa();
        }

        /// <summary>
        /// Using Aes 256, sets Aes key, generates new Rsa key with length 2048 bits
        /// </summary>
        /// <param name="aesKey">Aes key in byte[]</param>
        public CryptoClass(byte[] aesKey)
        {
            Aes = Aes.Create();
            Aes.KeySize = 256;

            Aes.Key = aesKey;
            GenerateKeyRsa();
        }

        /// <summary>
        /// Using Aes256, generate new key for Aes, sets key for Rsa from XML string
        /// </summary>
        /// <param name="rsaKey">XML string with Rsa key</param>
        public CryptoClass(string rsaKey)
        {
            Aes = Aes.Create();
            Aes.KeySize = 256;

            GenerateKeyAes();
            Rsa.FromXmlString(rsaKey);
        }

        /// <summary>
        /// Using Aes256,  sets Aes key, sets key for Rsa from XML string
        /// </summary>
        /// <param name="aesKey">Aes key in byte[]</param>
        /// <param name="rsaKey">XML string with Rsa key</param>
        public CryptoClass(byte[] aesKey, string rsaKey)
        {
            Aes = Aes.Create();
            Aes.KeySize = 256;

            Aes.Key = aesKey;
            Rsa.FromXmlString(rsaKey);
        }

        /// <summary>
        /// Encrypts info using Aes 256 existng key
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Encrypted info in byte[]</returns>
        public byte[] EncryptAes(byte[] data)
        {
            if (data == null)
                return new byte[0];

            using (var ms = new MemoryStream())
            {
                Aes.GenerateIV();
                var encryptor = Aes.CreateEncryptor(Aes.Key, Aes.IV);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(BitConverter.GetBytes(data.Length), 0, 4);
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    ms.Write(Aes.IV, 0, Aes.IV.Length);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Decrypts using Aes256 existing key
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Decrypted info in byte[]</returns>
        public byte[] DecryptAes(byte[] data)
        {
            byte[] IV = new byte[Aes.IV.Length];
            for (int i = data.Length - Aes.IV.Length; i < data.Length; i++)
            {
                IV[i - data.Length + Aes.IV.Length] = data[i];
            }

            using (var ms = new MemoryStream(data))
            {
                var decryptor = Aes.CreateDecryptor(Aes.Key, IV);
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (var br = new BinaryReader(cs))
                    {
                        int count = br.ReadInt32();
                        return br.ReadBytes(count);
                    }
                }
            }
        }

        /// <summary>
        /// Encrypt info using existing Rsa key
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Encrypted info in byte[]</returns>
        public byte[] EncryptRsa(byte[] data)
        {
            return Rsa.Encrypt(data, true);
        }

        /// <summary>
        /// Decrypts Rsa using existing key
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Decrypted info in byte[]</returns>
        public byte[] DecryptRsa(byte[] data)
        {
            return Rsa.Decrypt(data, true);
        }

        /// <summary>
        /// Generate new Aes key in 256 bits
        /// </summary>
        public void GenerateKeyAes()
        {
            Aes.GenerateKey();
        }

        /// <summary>
        /// Generates new key for Rsa with length 2048 bits
        /// </summary>
        public void GenerateKeyRsa()
        {
            if (Rsa != null)
                Rsa.Dispose();

            Rsa = new RSACryptoServiceProvider(2048);
        }

        /// <summary>
        /// Sets Rsa key from XML string
        /// </summary>
        /// <param name="key">XML string with Rsa key</param>
        public void SetRsaKey(string key)
        {
            Rsa.FromXmlString(key);
        }

        /// <summary>
        /// Sets Aes key
        /// </summary>
        /// <param name="key">Key length should be 32 bytes</param>
        public void SetAesKey(byte[] key)
        {
            Aes.Key = key;
        }

        /// <summary>
        /// Returns Rsa key in XML string
        /// </summary>
        /// <param name="includePrivateParametrs">True for private key, false for open</param>
        /// <returns>Rsa key in XML string</returns>
        public string GetRsaKey(bool includePrivateParametrs)
        {
            return Rsa.ToXmlString(includePrivateParametrs);
        }

        public byte[] GetAesKey()
        {
            return Aes.Key;
        }

        public void Dispose()
        {
            Rsa.Dispose();
            Aes.Dispose();
        }
    }
}
