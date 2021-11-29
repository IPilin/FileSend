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

        public byte[] AesKey { get; set; }

        public string RsaPrivateKey { get; set; }

        public string RsaPublicKey { get; set; }


        public CryptoClass()
        {
            Aes = Aes.Create("AES");

            AesKey = GenerateKey();

            RsaPrivateKey = GenerateKeyRsa()[0];
            RsaPublicKey = GenerateKeyRsa()[1];
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data == null)
                return new byte[0];

            using (var ms = new MemoryStream())
            {
                var encryptor = Aes.CreateEncryptor(AesKey, Aes.IV);
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

            byte[] IV = new byte[16];
            for (int i = data.Length - 16; i < data.Length; i++)
            {
                IV[i - data.Length + 16] = data[i];
            }

            using (var ms = new MemoryStream(data))
            {
                var decryptor = Aes.CreateDecryptor(AesKey, IV);
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

            Rsa.FromXmlString(RsaPublicKey);
            return Rsa.Encrypt(data, true);

        }

        public byte[] DecryptRsa(byte[] data)
        {
            if (data == null)
                return new byte[0];

            Rsa.FromXmlString(RsaPrivateKey);
            return Rsa.Decrypt(data, true);

        }

        public byte[] GenerateKey()
        {
            Aes.GenerateKey();
            return Aes.Key;
        }

        public string[] GenerateKeyRsa()
        {
            if (Rsa != null)
                Rsa.Dispose();

            Rsa = new RSACryptoServiceProvider(2048);

            string[] result = new string[2];
            result[0] = Rsa.ToXmlString(true);
            result[1] = Rsa.ToXmlString(false);

            return result;
        }

        public void Dispose()
        {
            Rsa.Dispose();
            Aes.Dispose();
        }
    }
}
