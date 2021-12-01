using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FileSend.Classes;
using System.Text;

namespace Tests
{
    [TestClass]
    public class CryptoTest
    {
        [TestMethod]
        public void AesTests()
        {
            CryptoClass crypto = new CryptoClass();
            
            var key1 = crypto.GetAesKey();
            crypto.GenerateKeyAes();
            var key2 = crypto.GetAesKey();

            Assert.IsTrue(!key1.Equals(key2), "Wrong Aes keys generator!");

            crypto.SetAesKey(key1);

            String message = "Hello, world";

            byte[] encryptData = crypto.EncryptAes(Encoding.UTF8.GetBytes(message));
            byte[] decryptData = crypto.DecryptAes(encryptData);

            Assert.AreEqual(message, Encoding.UTF8.GetString(decryptData), "Aes doesn't work!");
        }

        [TestMethod]
        public void RsaTests()
        {
            CryptoClass crypto = new CryptoClass();

            var key1 = crypto.GetRsaKey(true);
            crypto.GenerateKeyRsa();
            var key2 = crypto.GetRsaKey(true);

            Assert.IsTrue(!key1.Equals(key2), "Wrong Rsa keys generator!");

            crypto.SetRsaKey(key1);

            String message = "Hello, world";

            byte[] encryptData = crypto.EncryptRsa(Encoding.UTF8.GetBytes(message));
            byte[] decryptData = crypto.DecryptRsa(encryptData);

            Assert.AreEqual(message, Encoding.UTF8.GetString(decryptData), "Rsa doesn't work!");
        }
    }
}
