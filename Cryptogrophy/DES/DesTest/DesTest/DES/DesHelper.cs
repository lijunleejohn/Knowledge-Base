using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace DesTest.DES
{
    public static class DesHelper
    {
        /// <span class="code-SummaryComment"><summary></span>
        /// Encrypt a string from a Base64 string
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="originalString">The original string.</param></span>
        /// <span class="code-SummaryComment"><returns>The encrypted string.</returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">This exception will be </span>
        /// thrown when the original string is null or empty.<span class="code-SummaryComment"></exception></span>
        public static string EncryptString2String(string originalString, byte[] key, byte[] iv)
        {
            byte[] encrypted = DesCore.EncryptByte2Byte(Encoding.UTF8.GetBytes(originalString), key, iv);
            return Convert.ToBase64String(encrypted);
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Encrypt a byte array to a Base64 string.
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="originalImage">The original byte array.</param></span>
        /// <span class="code-SummaryComment"><returns>The encrypted string.</returns></span>
        public static string EncryptByte2String(byte[] originalImage, byte[] key, byte[] iv)
        {
            byte[] encrypted = DesCore.EncryptByte2Byte(originalImage, key, iv);
            return Convert.ToBase64String(encrypted);
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Decrypt a crypted string to string
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="cryptedString">The crypted string.</param></span>
        /// <span class="code-SummaryComment"><returns>The decrypted string.</returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">This exception will be thrown </span>
        /// when the crypted string is null or empty.<span class="code-SummaryComment"></exception></span>
        public static string DecryptString2String(string cryptedString, byte[] dKey, byte[] dIv)
        {
            byte[] decrypted = DesCore.DecryptByte2Byte(Convert.FromBase64String(cryptedString), dKey, dIv);
            return Encoding.UTF8.GetString(decrypted);
        }

        /// <span class="code-SummaryComment"><summary></span>
        /// Decrypt a crypted string back to an image byte array
        /// <span class="code-SummaryComment"></summary></span>
        /// <span class="code-SummaryComment"><param name="cryptedString">The crypted string.</param></span>
        /// <span class="code-SummaryComment"><returns>The decrypted byte array.</returns></span>
        /// <span class="code-SummaryComment"><exception cref="ArgumentNullException">This exception will be thrown </span>
        /// when the crypted string is null or empty.<span class="code-SummaryComment"></exception></span>
        public static byte[] DecryptString2Byte(string cryptedString, byte[] dKey, byte[] dIv)
        {
            byte[] decrypted = DesCore.DecryptByte2Byte(Convert.FromBase64String(cryptedString), dKey, dIv);
            return decrypted;
        }
    }
}
