using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KnowledgeBase.Crytography.Base64;

namespace Base64Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// <remarks>
    /// For Text demo: Encode uses self wrote Base64Algorithm.Base64Encoding(), Decode uses System.Convert.FromBase64String()
    /// For Image demo: Encode uses System.Convert.ToBase64String(), Decode uses self wrote Base64Algorithm.Base64Decode()
    /// </remarks>
    public partial class MainWindow : Window
    {
        private const string fileFolder = "c:\\temp\\";
        private string fileName = string.Empty;
        private string imageFileName = fileFolder + "Base64Image.txt";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnTextBase64Encode_Click(object sender, RoutedEventArgs e)
        {
            string base64String = new string(Base64Algorithm.Base64Encoding(Encoding.UTF8.GetBytes(txtOriginal.Text)));  //Encode uses self wrote Base64Algorithm.Base64Encoding()
            txtBase64Encoded.Text = base64String;
        }

        private void btnTextBase64Decode_Click(object sender, RoutedEventArgs e)
        {
            string originalString = new string(Encoding.UTF8.GetChars(System.Convert.FromBase64String(txtBase64Encoded.Text)));  //Decode uses System.Convert.FromBase64String()
            txtBase64Decode.Text = originalString;
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

        private void btnImageBase64Encode_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes = File.ReadAllBytes(fileName);
            string base64String = System.Convert.ToBase64String(imageBytes);    //Encode uses System.Convert.ToBase64String()
            txtImageBase64Encode.Text = base64String;
            File.WriteAllText(imageFileName, base64String);
        }

        private void btnImageBase64Decode_Click(object sender, RoutedEventArgs e)
        {
            byte[] imageBytes = Base64Algorithm.Base64Decoding(txtImageBase64Encode.Text.ToCharArray());    //Decode uses self wrote Base64Algorithm.Base64Decode()

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
            imgBase64Decoded.Source = image;
        }
    }
}
