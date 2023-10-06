using System.Windows;
using System.Windows.Forms;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Certificate
{
    /// <summary>
    /// Asymmetric encryption/decryption: everyone can encrypt (using a public key), only the host can decrypt (using a private key)
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fileFolder = "c:\\temp\\";
        private string fileName = string.Empty;
        private string password = "Test123";

        public MainWindow()
        {
            InitializeComponent();
            txtRSAClearText.Text = "Asymmetric encryption/decryption: everyone can encrypt (using a public key), only the host can decrypt (using a private key). Asymmetric signature/verification: only the host can sign (using a private key), everyone can verify (using a public key).\r\nImport the public certificate (*.cer) to encrypt, then import the private certificate(*.pfx) to decrypt.";
        }

        private void btnViewCertificate_Click(object sender, RoutedEventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(txtUrl.Text);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();

            //retrieve the ssl cert and assign it to an X509Certificate object
            X509Certificate cert = request.ServicePoint.Certificate;

            //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
            X509Certificate2 cert2 = new X509Certificate2(cert);

            ShowCertificateDetail(cert2);

            //display the cert dialog box
            X509Certificate2UI.DisplayCertificate(cert2);
        }

        private void btnImportCertificate_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = fileFolder;
            dlg.Filter = "Certificate files (*.cer)|*.cer|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = dlg.FileName;
                lblCertificateFileName.Content = fileName;

                X509Certificate2 cert2 = new X509Certificate2();
                cert2.Import(fileName, password, X509KeyStorageFlags.DefaultKeySet);

                ShowCertificateDetail(cert2);
                //display the cert dialog box
                X509Certificate2UI.DisplayCertificate(cert2);
            }
        }

        private void ShowCertificateDetail(X509Certificate2 cert2)
        {
            string cn = cert2.Issuer;
            string cpub = cert2.GetPublicKeyString();
            string thumbPrint = cert2.Thumbprint;

            StringBuilder sb = new StringBuilder();
            sb.Append("Cerntificate Name:");
            sb.AppendLine(cn);
            sb.AppendLine("-----------------------------------------------");
            sb.AppendLine("Public Key:");
            sb.AppendLine(cpub);
            sb.AppendLine("-----------------------------------------------");
            sb.Append("Thumb Print: ");
            sb.AppendLine(thumbPrint);
            sb.AppendLine("-----------------------------------------------");
            sb.Append("Calculated Thumb Print: ");
            sb.AppendLine(GetSha1Hash(cert2));
            txtCertificate.Text = sb.ToString();
        }

        private string GetSha1Hash(X509Certificate2 cert2)
        {
            SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider();
            byte[] result = SHA1.ComputeHash(cert2.RawData);

            StringBuilder sb = new StringBuilder();
            foreach(byte b in result)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString().ToUpper();
        }

        private void btnCertEncrypt_Click(object sender, RoutedEventArgs e)
        {
            //Load the certificate from a file
            X509Certificate2 cert2 = new X509Certificate2(fileName, password);
            using (RSA rsa = cert2.GetRSAPublicKey())
            {
                // OAEP allows for multiple hashing algorithms, what was formermly just "OAEP" is
                // now OAEP-SHA1.
                txtRSAEncrypted.Text = System.Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(txtRSAClearText.Text), RSAEncryptionPadding.OaepSHA1));
            }
        }

        private void btnCertDecrypt_Click(object sender, RoutedEventArgs e)
        {
            // GetRSAPrivateKey returns an object with an independent lifetime, so it should be
            // handled via a using statement.
            X509Certificate2 cert_public = new X509Certificate2(fileName, password, X509KeyStorageFlags.PersistKeySet);
            using (RSA rsa = cert_public.GetRSAPrivateKey())
            {
                byte[] result = rsa.Decrypt(System.Convert.FromBase64String(txtRSAEncrypted.Text), RSAEncryptionPadding.OaepSHA1);
                txtRSADecrypted.Text = Encoding.UTF8.GetString(result);
            }
        }
    }
}
