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
                return new byte[0];

            using (var aes = Aes.Create())
            {
                using (var ms = new MemoryStream())
                {                   
                    var encryptor = aes.CreateEncryptor(AesKey, aes.IV);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(BitConverter.GetBytes(data.Length), 0, 4);
                        cs.Write(data, 0, data.Length);
                        cs.FlushFinalBlock();
                        ms.Write(aes.IV, 0, aes.IV.Length);
                    }                    
                    return ms.ToArray();
                }
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
                using (Aes aes = Aes.Create())
                {
                    var decryptor = aes.CreateDecryptor(AesKey, IV);
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
