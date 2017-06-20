using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json;

namespace Forwarder_Server.Sources
{
    class Cryptography
    {
        private byte[] KEY;
        private byte[] IV;
        private RSAParameters privateKey;
        private RSAParameters publicKey;

        public String GenerateRSAKeys()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            this.privateKey = RSA.ExportParameters(true);
            this.publicKey = RSA.ExportParameters(false);
            return JsonConvert.SerializeObject(publicKey);
        }

        public String GenerateAESKey(String publicKey)
        {
            this.publicKey = JsonConvert.DeserializeObject<RSAParameters>(publicKey);

            Aes aes = Aes.Create();
            this.KEY = aes.Key;
            this.IV = aes.IV;

            byte[] encKey = RSAEncrypt(KEY, this.publicKey, false);
            byte[] encIV = RSAEncrypt(IV, this.publicKey, false);
            return JsonConvert.SerializeObject(new byte[][] { encKey, encIV });
        }

        public void GetAESKey(string keys)
        {
            byte[][] byteKeys = JsonConvert.DeserializeObject<byte[][]>(keys);
            this.KEY = RSADecrypt(byteKeys[0], privateKey, false);
            this.IV = RSADecrypt(byteKeys[1], privateKey, false);
        }

        public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.ImportParameters(RSAKeyInfo);
            return RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
        }

        public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.ImportParameters(RSAKeyInfo);
            return RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
        }

        public String GetHash(String[] input)
        {
            SHA256 mySHA256 = SHA256Managed.Create();
            String json = JsonConvert.SerializeObject(input);
            byte[] hashValue = mySHA256.ComputeHash(Encoding.Unicode.GetBytes(json));
            return JsonConvert.SerializeObject(hashValue);
        }


        public string Encrypt_AES_String(string input)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(input);

            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(KEY, IV);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string Decrypt_AES_String(string input)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(input);

            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(KEY, IV);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length + 1];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            return plainText;
        }
    }
}
