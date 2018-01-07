using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для DesAlg.xaml
    /// </summary>
    public partial class DesAlg : UserControl
    {
        private static int keyType;
        public static byte[] key;
        public static byte[] IV;
        public DesAlg()
        {
            InitializeComponent();
        }
        private void OnLoad(object sender, RoutedEventArgs e)
        {
            random.IsChecked = true;
        }
        private void KeyTypeChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).Name == "random")
            {
                keyType = 0;
                passBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                keyType = 1;
                passBlock.Visibility = Visibility.Visible;
            }
        }
        private void OpenPass(object sender, EventArgs e)
        {
            (passBox.Child as System.Windows.Forms.TextBox).PasswordChar = Char.MinValue;
            (passBox.Child as System.Windows.Forms.TextBox).Focus();
        }
        private void ClosePass(object sender, EventArgs e)
        {
            (passBox.Child as System.Windows.Forms.TextBox).PasswordChar = '•';
            (passBox.Child as System.Windows.Forms.TextBox).Focus();
        }
        private void GenClick(object sender, RoutedEventArgs e)
        {
            if (keyType == 0)
            {
                GenerateKey();
            }
            else
            {
                if ((passBox.Child as System.Windows.Forms.TextBox).Text == "")
                {
                    MessageBox.Show("Ви не ввели пароль!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Warning);
                    (passBox.Child as System.Windows.Forms.TextBox).Focus();
                    return;
                }
                GenerateKey((passBox.Child as System.Windows.Forms.TextBox).Text);
            }
        }
        public void GenerateKey(string pass)
        {
            byte[] salt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(pass, salt);
            key = rfc.GetBytes(8);
            GenerateKey();
        }
        public void GenerateKey()
        {
            RNGCryptoServiceProvider rNG = new RNGCryptoServiceProvider();
            if (keyType == 1)
            {
                IV = new byte[8];
                rNG.GetBytes(IV);
            }
            else
            {
                key = new byte[8];
                IV = new byte[8];
                rNG.GetBytes(key);
                rNG.GetBytes(IV);
            }
        }
        public BitmapSource RunDes(BitmapSource inputImg, int action)
        {
            if (key == null || IV == null)
                return null;
            
            int bytesPerPixel = (inputImg.Format.BitsPerPixel + 7) / 8;
            byte[] bytes = new byte[inputImg.PixelWidth * inputImg.PixelHeight * bytesPerPixel];
            inputImg.CopyPixels(bytes, inputImg.PixelWidth * bytesPerPixel, 0);
            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = key;
                des.IV = IV;
                des.Padding = PaddingMode.None;
                if(action == 0)
                {
                    bytes = EncryptImg(des, bytes);
                    return BitmapSource.Create(inputImg.PixelWidth, inputImg.PixelHeight, inputImg.DpiX, inputImg.DpiY, inputImg.Format, inputImg.Palette, bytes, inputImg.PixelWidth * bytesPerPixel);
                }
                else
                {
                    bytes = DecryptImg(des, bytes);
                    return BitmapSource.Create(inputImg.PixelWidth, inputImg.PixelHeight, inputImg.DpiX, inputImg.DpiY, inputImg.Format, inputImg.Palette, bytes, inputImg.PixelWidth * bytesPerPixel);
                }   
            }
        }
        private byte[] EncryptImg(SymmetricAlgorithm alg, byte[] pixels)
        {
            using (var stream = new MemoryStream())
            using (var encryptor = alg.CreateEncryptor())
            using (var encrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
            {
                encrypt.Write(pixels, 0, pixels.Length);
                encrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }
        private byte[] DecryptImg(SymmetricAlgorithm alg, byte[] pixels)
        {
            using (var stream = new MemoryStream())
            using (var decryptor = alg.CreateDecryptor())
            using (var decrypt = new CryptoStream(stream, decryptor, CryptoStreamMode.Write))
            {
                decrypt.Write(pixels, 0, pixels.Length);
                decrypt.FlushFinalBlock();
                return stream.ToArray();
            }
        }
        public string ManageExport()
        {
            string export = "1;" + String.Join(",", IV) + ";";
            if (keyType == 0)
                export = export + "0;" + String.Join(",", key);
            else
                export = export + "1";
           
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(export));
        }
        
    }
}
