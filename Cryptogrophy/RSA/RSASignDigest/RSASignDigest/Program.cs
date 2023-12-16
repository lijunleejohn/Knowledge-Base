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

            Console.WriteLine($"Public Key:\r\n{publicKey}\r\n");
            Console.WriteLine($"Private Key:\r\n{privateKey}\r\n");

            // create the signature using the private key
            rsa.FromXmlString(privateKey);
            byte[] signature = rsa.SignData(message, SHA1.Create());

            // verify the signature using the public key
            rsa.FromXmlString(publicKey);
            if (rsa.VerifyData(message, SHA1.Create(), signature))
            {
                Console.WriteLine("The message is correctly signed using private key and verifed by public key\r\n");
            }

            //Create a UnicodeEncoder to convert between byte array and string.
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            String originalData = "Data to Encrypt";

            //Create byte arrays to hold original, encrypted, and decrypted data.
            byte[] dataToEncrypt = ByteConverter.GetBytes(originalData);
            byte[] encryptedData;
            byte[] decryptedData;

            // Use public key to encript
            //Display the key-legth to the console.
            Console.WriteLine("A new key pair of length {0} was created\r\n", rsa.KeySize);
            Console.WriteLine($"The original text is '{originalData}'\r\n");

            //Pass the data to ENCRYPT, the public key information
            //(using RSACryptoServiceProvider.ExportParameters(false),
            //and a boolean flag specifying no OAEP padding.
            encryptedData = RSAEncrypt(dataToEncrypt, rsa.ExportParameters(false), false);

            Console.WriteLine($"Encrypted data in String format:\r\n{ByteConverter.GetString(encryptedData)}\r\n");
            Console.WriteLine($"Encrypted data in Base64 format:\r\n{System.Convert.ToBase64String(encryptedData)}\r\n");
            // Use private key to decript
            //Pass the data to DECRYPT, the private key information
            //(using RSACryptoServiceProvider.ExportParameters(true),
            //and a boolean flag specifying no OAEP padding.
            rsa.FromXmlString(privateKey);
            decryptedData = RSADecrypt(encryptedData, rsa.ExportParameters(true), false);

            //Display the decrypted plaintext to the console.
            Console.WriteLine("Decrypted plaintext: '{0}'\r\n", ByteConverter.GetString(decryptedData));

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

        static public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                //Create a new instance of RSACryptoServiceProvider.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                //Import the RSA Key information. This only needs
                //toinclude the public key information.
                RSAalg.ImportParameters(RSAKeyInfo);

                //Encrypt the passed byte array and specify OAEP padding.
                //OAEP padding is only available on Microsoft Windows XP or
                //later.
                return RSAalg.Encrypt(DataToEncrypt, DoOAEPPadding);
            }
            //Catch and display a CryptographicException
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }
        }

        static public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                //Create a new instance of RSACryptoServiceProvider.
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();

                //Import the RSA Key information. This needs
                //to include the private key information.
                RSAalg.ImportParameters(RSAKeyInfo);

                //Decrypt the passed byte array and specify OAEP padding.
                //OAEP padding is only available on Microsoft Windows XP or
                //later.
                return RSAalg.Decrypt(DataToDecrypt, DoOAEPPadding);
            }
            //Catch and display a CryptographicException
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }
        }
    }
}
