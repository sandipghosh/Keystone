
namespace Keystone.Web.Security
{
    using System;
    using System.IO;
    using System.Text;
    using System.Security.Cryptography;
    public class Crypto
    {
        private const string PASS_PHRASE = "Pas5pr@se"; //can be any string
        private const string SALT_VALUE = "s@1tValue"; //can be any string
        private const string HASH_ALGORITHM = "SHA1"; //can be "MD5"
        private const int PASSWORD_ITERATIONS = 2; //can be any number
        private const string INIT_VECTOR = "@1B2c3D4e5F6g7H8"; //must be 16 bytes
        private const int KEY_SIZE = 256; //can be 128, 192 or 256 bits 

        /// <summary>
        /// Encrypts the specified plain text.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string salt = SALT_VALUE)
        {
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(INIT_VECTOR);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                PasswordDeriveBytes password = new PasswordDeriveBytes(PASS_PHRASE, saltValueBytes, HASH_ALGORITHM, PASSWORD_ITERATIONS);
                byte[] keyBytes = password.GetBytes(KEY_SIZE / 8);

                // Create uninitialized Rijndael encryption object.
                RijndaelManaged symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC };

                using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                            cryptoStream.FlushFinalBlock();
                            return Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //CrossCutting.Utility.LogToFile(ex.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// Decrypts the specified cipher text.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <returns></returns>
        public static string Decrypt(string cipherText, string salt = SALT_VALUE)
        {
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(INIT_VECTOR);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(salt);
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText.Replace(" ", "+"));

                PasswordDeriveBytes password = new PasswordDeriveBytes(PASS_PHRASE, saltValueBytes, HASH_ALGORITHM, PASSWORD_ITERATIONS);
                byte[] keyBytes = password.GetBytes(KEY_SIZE / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC };

                using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                {
                    using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            byte[] plainTextBytes = new byte[cipherTextBytes.Length + 1];
                            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //CrossCutting.Utility.LogToFile(ex.Message);
            }
            return string.Empty;
        }
    }
}