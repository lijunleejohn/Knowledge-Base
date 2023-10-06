using System;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string fileFolder = "c:\\temp\\";
        private string fileName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void HashBytes(byte[] data)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            txtMD5.Text = Convert.ToBase64String(MD5.ComputeHash(data));

            SHA1CryptoServiceProvider SHA1 = new SHA1CryptoServiceProvider();
            txtSHA1.Text = Convert.ToBase64String(SHA1.ComputeHash(data));

            SHA256Managed SHA256 = new SHA256Managed();
            txtSHA256.Text = Convert.ToBase64String(SHA256.ComputeHash(data));

            SHA512Managed SHA512 = new SHA512Managed();
            txtSHA512.Text = Convert.ToBase64String(SHA512.ComputeHash(data));
        }

        private void btnHashText_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = Encoding.UTF8.GetBytes(txtClearText.Text);

            HashBytes(data);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

        private void btnHashImage_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes = File.ReadAllBytes(fileName);
            HashBytes(imageBytes);
        }
    }
}
