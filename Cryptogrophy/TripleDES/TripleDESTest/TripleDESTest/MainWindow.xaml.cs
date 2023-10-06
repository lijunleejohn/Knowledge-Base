using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using TripleDESTest.TripleDES;

namespace DesTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fileFolder = "c:\\temp\\";
        private string fileName = string.Empty;
        private string imageFileName = fileFolder + "DesImage.des.binary";
        private static string keyValue = "l:[}lH&^f%4D$#saS#)A?.<]";
        private static string initialVector = "OneCloud";

        //TripleDESCryptoServiceProvider algorithm supports key lengths from 128 bits to 192 bits in increments of 64 bits.
        static byte[] key = ASCIIEncoding.UTF8.GetBytes(keyValue);

        //The IV property is automatically set to a new random value whenever you create a new instance of one of the SymmetricAlgorithm classes or when you manually call the GenerateIV method.
        //The size of the IV property must be the same as the BlockSize property.
        static byte[] iv = ASCIIEncoding.UTF8.GetBytes(initialVector);
        static byte[] dKey, dIv;

        public MainWindow()
        {
            InitializeComponent();
            txtKey.Text = keyValue;
            txtIv.Text = initialVector;
        }

        private void btnTextDesEncode_Click(object sender, RoutedEventArgs e)
        {
            txtDesEncoded.Text = TripleDESHelper.EncryptString2String(txtOriginal.Text, key, iv);
        }

        private void btnTextDesDecode_Click(object sender, RoutedEventArgs e)
        {
            dKey = ASCIIEncoding.ASCII.GetBytes(txtKey.Text);
            dIv = ASCIIEncoding.ASCII.GetBytes(txtIv.Text);
            txtDesDecode.Text = TripleDESHelper.DecryptString2String(txtDesEncoded.Text, dKey, dIv);
        }

        private void btnBrowseImageFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = fileFolder;
            dlg.Filter = "Image files (*.jpg, *.png)|*.jpg;*.png|All Files (*.*)|*.*";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = dlg.FileName;
                lblFileName.Content = fileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fileName);
                bitmap.EndInit();
                imgOriginal.Source = bitmap;
            }
        }

        private void btnImageDesEncode_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes = File.ReadAllBytes(fileName);

            txtImageDesEncode.Text = TripleDESHelper.EncryptByte2String(imageBytes, key, iv);

            //write byte array into a file
            File.WriteAllBytes(imageFileName, TripleDESCore.EncryptByte2Byte(imageBytes, key, iv));
        }

        private void btnImageDesDecode_Click(object sender, RoutedEventArgs e)
        {
            dKey = ASCIIEncoding.ASCII.GetBytes(txtKey.Text);
            dIv = ASCIIEncoding.ASCII.GetBytes(txtIv.Text);
            byte[] imageBytes = TripleDESHelper.DecryptString2Byte(txtImageDesEncode.Text, dKey, dIv);    //Decode uses self wrote Base64Algorithm.Base64Decode()

            if (imageBytes == null || imageBytes.Length == 0) return;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageBytes))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            imgDesDecoded.Source = image;
        }

        private void btnLoadDesBinFile_Click(object sender, RoutedEventArgs e)
        {
            dKey = ASCIIEncoding.ASCII.GetBytes(txtKey.Text);
            dIv = ASCIIEncoding.ASCII.GetBytes(txtIv.Text);
            byte[] imageBytes = TripleDESCore.DecryptByte2Byte(File.ReadAllBytes(imageFileName), dKey, dIv);    //Decode uses self wrote Base64Algorithm.Base64Decode()

            if (imageBytes == null || imageBytes.Length == 0) return;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageBytes))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            imgDesDecoded.Source = image;
        }
    }
}
