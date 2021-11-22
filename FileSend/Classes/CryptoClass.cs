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
        public byte[] EncryptedMessage { get; private set; }
        public byte[] PureMessage { get; private set; }

        private RSACryptoServiceProvider Key;

        CryptoClass(byte[] message, RSACryptoServiceProvider key)
        { 
        }
    }
}
