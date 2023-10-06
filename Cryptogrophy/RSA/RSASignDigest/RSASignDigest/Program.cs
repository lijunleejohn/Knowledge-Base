using System;
using System.Text;
using System.Security.Cryptography;

namespace RSASignDigest
{
    /// <summary>
    /// Asymmetric signature/verification: only the host can sign (using a private key), everyone can verify (using a public key).
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            byte[] message = Encoding.UTF8.GetBytes("Fair winds and following seas.");

            // calculate keys
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string publicKey = rsa.ToXmlString(false);
            string privateKey = rsa.ToXmlString(true);

            // create the signature using the private key
            rsa.FromXmlString(privateKey);
            byte[] signature = rsa.SignData(message, new SHA1CryptoServiceProvider());

            // verify the signature using the public key
            rsa.FromXmlString(publicKey);
            if (rsa.VerifyData(message, new SHA1CryptoServiceProvider(), signature))
            {
                Console.WriteLine("The message is correctly signed using private key and verifed by public key");
            }

            // THe following code block will throw exception: Internal.Cryptography.CryptoThrowHelper.WindowsCryptographicException: 'Keyset does not exist'
            // This is because only Private Key can sign a digest and claim who it is, public key can be used by anybody
            /*
            // create the signature using the public key
            rsa.FromXmlString(publicKey);
            signature = rsa.SignData(message, new SHA1CryptoServiceProvider());

            // verify the signature using the private key
            rsa.FromXmlString(privateKey);
            if (rsa.VerifyData(message, new SHA1CryptoServiceProvider(), signature))
            {
                Console.WriteLine("The message is correctly signed using public key and verified by private key");
            }
            */
        }
    }
}
