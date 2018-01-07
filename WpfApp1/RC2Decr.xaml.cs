﻿using System;
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
    /// Логика взаимодействия для RC2Decr.xaml
    /// </summary>
    public partial class RC2Decr : UserControl
    {
        public static int keySize;
        public static byte[] key;
        public static byte[] IV;
        public RC2Decr()
        {
            InitializeComponent();
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
        public void Generate(object sender, RoutedEventArgs e)
        {
            byte[] salt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes((passBox.Child as System.Windows.Forms.TextBox).Text, salt);
            key = rfc.GetBytes(keySize / 8);
        }
        public BitmapSource DecryptImg(BitmapSource inputImg)
        {
            if (key == null || IV == null)
                return null;

            int bytesPerPixel = (inputImg.Format.BitsPerPixel + 7) / 8;
            byte[] bytes = new byte[inputImg.PixelWidth * inputImg.PixelHeight * bytesPerPixel];
            inputImg.CopyPixels(bytes, inputImg.PixelWidth * bytesPerPixel, 0);
            using (var aes = new RC2CryptoServiceProvider())
            {
                aes.Key = key;
                aes.IV = IV;
                aes.Padding = PaddingMode.None;
                bytes = Decrypt(aes, bytes);
                return BitmapSource.Create(inputImg.PixelWidth, inputImg.PixelHeight, inputImg.DpiX, inputImg.DpiY, inputImg.Format, inputImg.Palette, bytes, inputImg.PixelWidth * bytesPerPixel);
            }
        }
        private byte[] Decrypt(SymmetricAlgorithm alg, byte[] pixels)
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
        public bool ReadKey(string path)
        {
            string unlockString = "";
            using (StreamReader file = new StreamReader(path, true))
            {
                unlockString = Encoding.Unicode.GetString(Convert.FromBase64String(file.ReadLine()));
                string[] keys = unlockString.Split(';');
                if (keys[0] != "4")
                {

                    return false;
                }
                keySize = Int32.Parse(keys[1]);
                IV = keys[2].Split(',').Select(Byte.Parse).ToArray();
                if (keys[3] == "0")
                {
                    key = keys[4].Split(',').Select(Byte.Parse).ToArray();
                    return false;
                }
                else
                    return true;
            }
        }
    }
}
