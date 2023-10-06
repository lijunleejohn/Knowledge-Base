using System;
using System.IO;
using System.Security.Cryptography;

namespace DesTest.DES
{
    public static class DesCore
    {
        /// <span class="code-SummaryComment"><summary></span>
        /// EncryptString2String a byte array.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="originalImage">The original byte array.</param></span>
        /// <span class="code-SummaryComment"><returns>The encrypted byte array.</returns></span>
        /// <remarks>
        /// DES Algorithm:
        /// Block Size: 64 bit (56 bit)
        /// Key Size: 64 bit
        /// Initial Vector: 64 bit
        /// </remarks>
        public static byte[] EncryptByte2Byte(byte[] originalImage, byte[] key, byte[] iv)
        {
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            cryptoProvider.Mode = CipherMode.CBC; //actually the default mode of SymmetricAlgorithm.Mode Property is "CBC"
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                cryptoProvider.CreateEncryptor(key, iv), CryptoStreamMode.Write);
            BinaryWriter writer = new BinaryWriter(cryptoStream);
            writer.Write(originalImage);
            writer.Flush();
            cryptoStream.FlushFinalBlock();
            writer.Flush();
            byte[] result = memoryStream.ToArray();
            return result;
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// DecryptString2String a crypted binary array back to an image byte array
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="cryptedBytes">The crypted binary array.</param></span>
        /// <span class="code-SummaryComment"><returns>The decrypted binary array.</returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">This exception will be thrown </span>
        /// when there is an error.<span class="code-SummaryComment"></exception></span>
        public static byte[] DecryptByte2Byte(byte[] cryptedBytes, byte[] dKey, byte[] dIv)
        {
            try
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream
                        (cryptedBytes);

                CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    cryptoProvider.CreateDecryptor(dKey, dIv), CryptoStreamMode.Read);
                BinaryReader reader = new BinaryReader(cryptoStream);
                return reader.ReadBytes(cryptedBytes.Length);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
