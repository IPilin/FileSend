using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace FileSend.Classes
{
    public class CryptoClass
    {
        public byte[] AesKey { get; set; }


        public CryptoClass()
        {
            AesKey = GenerateKey();
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data == null)
                return null;

            if (AesKey == null)
            {

            }

            byte[] toEncrypt;

            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    bw.Write(data.Length);
                    bw.Write(data);
                }

                toEncrypt = ms.ToArray();
            }

            using (Aes aes = Aes.Create())
            {
                byte[] encryptedData;
                aes.GenerateIV();

                ICryptoTransform encryptor = aes.CreateEncryptor(AesKey, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(toEncrypt, 0, toEncrypt.Length); 
                    }

                    encryptedData = ms.ToArray();
                }

                byte[] result = new byte[encryptedData.Length + aes.IV.Length];

                using (var ms = new MemoryStream(result))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    ms.Write(encryptedData, 0, encryptedData.Length);
                    ms.Write(aes.IV, 0, aes.IV.Length);

                    return ms.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            if (data == null)
                return null;

            byte[] IV;
            byte[] toDecrypt;
            byte[] decrypted;

            using (var ms = new MemoryStream(data))
            {
                using (var br = new BinaryReader(ms))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    toDecrypt = br.ReadBytes(data.Length - 16);
                    IV = br.ReadBytes(16);
                }
            }

            using (Aes aes = Aes.Create())
            {
                ICryptoTransform decryptor = aes.CreateDecryptor(AesKey, IV);

                using (var ms = new MemoryStream(toDecrypt))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        cs.Read(toDecrypt, 0, toDecrypt.Length);
                    }

                    decrypted = ms.ToArray();
                }
            }

            using (var ms = new MemoryStream(decrypted))
            {
                using (var br = new BinaryReader(ms))
                {
                    int toRead = br.ReadInt32();

                    return br.ReadBytes(toRead);
                }
            }
        }

        public byte[] GenerateKey()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();

                return aes.Key;
            }
        }
    }
}
