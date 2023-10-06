using System;
using System.IO;
using System.Security.Cryptography;

namespace AESTest.AES
{
    public static class AESCore
    {
        /// <span class="code-SummaryComment"><summary></span>
        /// EncryptString2String a byte array.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="originalImage">The original byte array.</param></span>
        /// <span class="code-SummaryComment"><returns>The encrypted byte array.</returns></span>
        /// <remarks>
        /// Triple DES Algorithm:
        /// Block Size: 128 bit
        /// Key Size: 128 bits
        /// IV Size: 128 bit 
        /// </remarks>
        public static byte[] EncryptByte2Byte(byte[] originalImage, byte[] key, byte[] iv)
        {
            // AESCryptoServiceProvider algorithm supports key lengths from 128 bits to 192 bits in increments of 64 bits.
            AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider();
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
        /// when there is an error.<span class="code-SummaryComment"></excep                                                                                          tion></span>
        public static byte[] DecryptByte2Byte(byte[] cryptedBytes, byte[] dKey, byte[] dIv)
        {
            try
            {
                AesCryptoServiceProvider cryptoProvider = new AesCryptoServiceProvider();
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
