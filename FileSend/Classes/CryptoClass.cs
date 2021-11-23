using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace FileSend.Classes
{
    class CryptoClass
    {
        public byte[] AesKey { get; set; }


        CryptoClass()
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

            using (Aes aes = Aes.Create())
            {
                aes.GenerateIV();

                ICryptoTransform cryptoTransform = aes.CreateEncryptor(AesKey, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write))
                    {
                        ms.Write(data, 0, data.Length);

                        return ms.ToArray();
                    }
                }
            }
        }

        public byte[] Decrypt(byte[] data)
        {
            if (data == null)
                return null;

            using (Aes aes = Aes.Create())
            {
                ICryptoTransform cryptoTransform = aes.CreateDecryptor(AesKey, null);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Read))
                    {
                        cs.Read(data, 0, data.Length);

                        return ms.ToArray();
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
