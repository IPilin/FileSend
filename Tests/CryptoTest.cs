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
            
            var key1 = crypto.GenerateKey();
            var key2 = crypto.GenerateKey();

            Assert.IsTrue(!key1.Equals(key2), "Wrong keys generator!");

            crypto.AesKey = key1;

            String message = "Hello, world";

            byte[] encryptData = crypto.Encrypt(Encoding.UTF8.GetBytes("Hello, world!"));
            byte[] decryptData = crypto.Decrypt(encryptData);

            Assert.AreEqual(message, Encoding.UTF8.GetString(decryptData), "Encrypt and Decrypt aren't working!");
        }
    }
}
