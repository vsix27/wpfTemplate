using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace EncryptUtility
{
    internal class CryptoHelper
    {
        private static string _sharedString;
        private static byte[] _salt;
        //internal static string SharedString = "VsIx!clandestine$2015";
        /// <summary>
        /// A password used to generate a key for encryption.
        /// </summary>
        internal static string SharedString
        {
            get
            {
                if (string.IsNullOrEmpty(_sharedString))
                    _sharedString = ConfigurationSettings.AppSettings.Get("SharedString");
                return _sharedString;
            }
        }

        //internal static byte[] Salt = Encoding.ASCII.GetBytes("$vicissitude!it");
        internal static byte[] Salt
        {
            get
            {
                if (_salt == null)
                    _salt = Encoding.ASCII.GetBytes("" + ConfigurationSettings.AppSettings.Get("Salt"));
                return _salt;
            }
        }

        internal static string Sugar { get { return Encoding.UTF8.GetString(Salt); } }
        
        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using DecryptStringAES().  The sharedSecret parameters must match.
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        public static string EncryptStringAes(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) throw new ArgumentNullException("plainText");

            string outStr = null; // Encrypted string to return
            RijndaelManaged aesAlg = null; // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(SharedString, Salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (var msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt)) swEncrypt.Write(plainText);
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

       

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using EncryptStringAES(), using an identical sharedSecret.
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        public static string DecryptStringAes(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) throw new ArgumentNullException("cipherText");

            // Declare the RijndaelManaged object used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                var key = new Rfc2898DeriveBytes(SharedString, Salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        // Read the decrypted bytes from the decrypting stream and place them in a string.
                        using (var srDecrypt = new StreamReader(csDecrypt)) plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null) aesAlg.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
