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
        /// Using Aes56, Rsa key length is 2048 bits
        /// </summary>
        public CryptoClass()
        {
            Aes = Aes.Create();
            Aes.KeySize = 256;

            GenerateKey();
            GenerateKeyRsa(true);
        }

        public byte[] Encrypt(byte[] data)
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

        public byte[] Decrypt(byte[] data)
        {
            if (data == null)
                return new byte[0];

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

        public byte[] EncryptRsa(byte[] data)
        {
            if (data == null)
                return new byte[0];

            return Rsa.Encrypt(data, true);
        }

        public byte[] DecryptRsa(byte[] data)
        {
            if (data == null)
                return new byte[0];

            return Rsa.Decrypt(data, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Aes key in byte[32]</returns>
        public byte[] GenerateKey()
        {
            Aes.GenerateKey();
            return Aes.Key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="includePrivateParametrs">True for private key, false for open</param>
        /// <returns>Rsa key in XML string</returns>
        public string GenerateKeyRsa(bool includePrivateParametrs)
        {
            if (Rsa != null)
                Rsa.Dispose();

            Rsa = new RSACryptoServiceProvider(2048);
            return Rsa.ToXmlString(includePrivateParametrs);
        }

        public void SetRsaKey(string key)
        {
            Rsa.FromXmlString(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key">Key length should be 32 bytes</param>
        /// <returns>0 if key changed</returns>
        public int SetAesKey(byte[] key)
        {
            if (key.Length != 32)
                return 1;

            Aes.Key = key;
            return 0;
        }

        /// <summary>
        /// Returns Rsa key in XML string
        /// </summary>
        /// <param name="includePrivateParametrs">True for private key, false for open</param>
        /// <returns></returns>
        public string RsaKey(bool includePrivateParametrs)
        {
            return Rsa.ToXmlString(includePrivateParametrs);
        }

        public byte[] AesKey()
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
