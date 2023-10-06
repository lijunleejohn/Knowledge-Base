using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using DesTest.DES;

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
        private static string keyValue = "ZeroCool";
        private static string initialVector = "OneCloud";

        static byte[] key = ASCIIEncoding.ASCII.GetBytes(keyValue); 
        static byte[] iv = ASCIIEncoding.ASCII.GetBytes(initialVector);
        static byte[] dKey, dIv;

        public MainWindow()
        {
            InitializeComponent();
            txtKey.Text = keyValue;
            txtIv.Text = initialVector;
        }

        private void btnTextDesEncode_Click(object sender, RoutedEventArgs e)
        {
            txtDesEncoded.Text = DesHelper.EncryptString2String(txtOriginal.Text, key, iv);
        }

        private void btnTextDesDecode_Click(object sender, RoutedEventArgs e)
        {
            dKey = ASCIIEncoding.ASCII.GetBytes(txtKey.Text);
            dIv = ASCIIEncoding.ASCII.GetBytes(txtIv.Text);
            txtDesDecode.Text = DesHelper.DecryptString2String(txtDesEncoded.Text, dKey, dIv);
        }

        private void btnBrowseImageFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = fileFolder;
            dlg.Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*";
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

            txtImageDesEncode.Text = DesHelper.EncryptByte2String(imageBytes, key, iv);

            //write byte array into a file
            File.WriteAllBytes(imageFileName, DesCore.EncryptByte2Byte(imageBytes, key, iv));
        }

        private void btnImageDesDecode_Click(object sender, RoutedEventArgs e)
        {
            dKey = ASCIIEncoding.ASCII.GetBytes(txtKey.Text);
            dIv = ASCIIEncoding.ASCII.GetBytes(txtIv.Text);
            byte[] imageBytes = DesHelper.DecryptString2Byte(txtImageDesEncode.Text, dKey, dIv);    //Decode uses self wrote Base64Algorithm.Base64Decode()

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
            byte[] imageBytes = DesCore.DecryptByte2Byte(File.ReadAllBytes(imageFileName), dKey, dIv);    //Decode uses self wrote Base64Algorithm.Base64Decode()

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
