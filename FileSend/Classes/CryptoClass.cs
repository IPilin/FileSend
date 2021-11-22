using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;


namespace FileSend.Classes
{
    class CryptoClass
    {
        public byte[] Key { get; set; }

        CryptoClass()
        {
            Key = GenerateKey();
        }

        public byte[] Encrypt(byte[] data)
        {
            return null;
        }

        public byte[] Decrypt(byte[] data)
        {
            return null;
        }

        public byte[] GenerateKey()
        {
            return null;
        }
    }
}
