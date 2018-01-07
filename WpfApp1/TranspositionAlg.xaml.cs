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
    /// Логика взаимодействия для TranspositionAlg.xaml
    /// </summary>
    
    #region ENCRYPT_CLASS
    public class Encrypt
    {
        public static List<string> test;
        public static int lenth = 256;
        public static List<byte[]> shuffledBlocks;
        public static byte[] alphaK;
        public static byte[] redK;
        public static byte[] greenK;
        public static byte[] blueK;
        public static List<int> ways;
        public static List<int> IV;
        public static List<int> alphaWays;
        public static List<int> redWays;
        public static List<int> greenWays;
        public static List<int> blueWays;
        private static bool resized = false;
        public static List<int> exportInfo;
        public static int bytesPerPixel;
        public static int method;
        public static bool inBlock = true;
        public static BitmapSource EncryptImg(BitmapSource inputImg, int method)
        {
            if (inBlock && alphaK.Length != lenth)
            {
                MessageBox.Show("Довжина згенерованих ключів менша за розміри блока!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
            exportInfo = new List<int>();
            exportInfo.Add(lenth);
            resized = false;
            bytesPerPixel = (inputImg.Format.BitsPerPixel + 7) / 8;
            if ((inputImg.PixelWidth % lenth) != 0 || (inputImg.PixelHeight % lenth) != 0)
            {
                exportInfo.Add(inputImg.PixelWidth);
                exportInfo.Add(inputImg.PixelHeight);
                int Width = 0, Height = 0;
                resized = true;
                if (inputImg.PixelWidth > inputImg.PixelHeight)
                {
                    if ((inputImg.PixelWidth % lenth) != 0)
                    {
                        int newSize = (inputImg.PixelWidth / lenth + 1) * lenth;
                        Width = newSize;
                        Height = newSize;
                    }
                    else
                    {
                        Width = inputImg.PixelWidth;
                        Height = inputImg.PixelWidth;
                    }
                }
                else if (inputImg.PixelWidth < inputImg.PixelHeight)
                {
                    if ((inputImg.PixelHeight % lenth) != 0)
                    {
                        int newSize = (inputImg.PixelHeight / lenth + 1) * lenth;
                        Width = newSize;
                        Height = newSize;
                    }
                    else
                    {
                        Width = inputImg.PixelHeight;
                        Height = inputImg.PixelHeight;
                    }
                }
                else if (inputImg.PixelWidth == inputImg.PixelHeight)
                {
                    int newSize = (inputImg.PixelWidth / lenth + 1) * lenth;
                    Width = newSize;
                    Height = newSize;
                }
                int neededBytes = Width * Height * bytesPerPixel;
                byte[] originalPixels = new byte[inputImg.PixelWidth * inputImg.PixelHeight * bytesPerPixel];
                inputImg.CopyPixels(originalPixels, inputImg.PixelWidth * bytesPerPixel, 0);
                byte[] pixels = new byte[neededBytes];
                int row = 0, col = 0, iteration = 1;
                for (int i = 0; i < originalPixels.Length; i++)
                {
                    pixels[row * Width * bytesPerPixel + col] = originalPixels[i];
                    col++;
                    if (i == (inputImg.PixelWidth * iteration * bytesPerPixel) - 1)
                    {
                        row++;
                        iteration++;
                        col = 0;
                    }
                }
                BitmapSource resizedImg = BitmapSource.Create(Width, Height, inputImg.DpiX, inputImg.DpiY, inputImg.Format, inputImg.Palette, pixels, Width * bytesPerPixel);
                inputImg = resizedImg;
            }

            List<byte[]> blocks = new List<byte[]>();
            int blocksCount = (inputImg.PixelWidth * inputImg.PixelHeight) / (int)(Math.Pow(lenth, 2));
            IV = GetWays(0, lenth, lenth);
            for (int yI = 0; yI < inputImg.PixelHeight / lenth; yI++)
            {
                for (int xI = 0; xI < inputImg.PixelWidth / lenth; xI++)
                {
                    byte[] pixels = new byte[(int)(Math.Pow(lenth, 2)) * bytesPerPixel];
                    Int32Rect rect = new Int32Rect(xI * lenth, yI * lenth, lenth, lenth);
                    inputImg.CopyPixels(rect, pixels, lenth * bytesPerPixel, 0);
                    if (inBlock)
                        pixels = EncryptBlocks(pixels);
                    blocks.Add(pixels);
                }
            }
            Random rng = new Random();
            int n = blocks.Count;
            ways = new List<int>();
            while (n > 1)
            {
                int k = rng.Next(n--);
                ways.Add(k);
                byte[] temp = blocks[n];
                blocks[n] = blocks[k];
                blocks[k] = temp;
            }
            var result = new WriteableBitmap(inputImg);
            int blockI = 0;
            for (int yI = 0; yI < result.PixelHeight / lenth; yI++)
            {
                for (int xI = 0; xI < result.PixelWidth / lenth; xI++)
                {
                    Int32Rect rect = new Int32Rect(xI * lenth, yI * lenth, lenth, lenth);
                    result.WritePixels(rect, blocks[blockI], lenth * bytesPerPixel, 0);
                    blockI++;
                }
            }
            return result;
        }
        public static BitmapSource InversEncryption(BitmapSource inputImg)
        {
            int bytesPerPixel = (inputImg.Format.BitsPerPixel + 7) / 8;
            List<byte[]> blocks = new List<byte[]>();
            int blocksCount = (inputImg.PixelWidth * inputImg.PixelHeight) / (int)(Math.Pow(lenth, 2));
            for (int yI = 0; yI < inputImg.PixelHeight / lenth; yI++)
            {
                for (int xI = 0; xI < inputImg.PixelWidth / lenth; xI++)
                {
                    byte[] pixels = new byte[(int)(Math.Pow(lenth, 2)) * bytesPerPixel];
                    Int32Rect rect = new Int32Rect(xI * lenth, yI * lenth, lenth, lenth);
                    inputImg.CopyPixels(rect, pixels, lenth * bytesPerPixel, 0);
                    if(inBlock)
                        pixels = DecryptBlocks(pixels);
                    blocks.Add(pixels);
                }
            }
            int n = 1;
            while (n < blocks.Count)
            {
                int k = ways[(blocks.Count - 1) - n];
                byte[] temp = blocks[n];
                blocks[n] = blocks[k];
                blocks[k] = temp;
                n++;
            }
            
            var result = new WriteableBitmap(inputImg);
            int blockI = 0;
            for (int yI = 0; yI < result.PixelHeight / lenth; yI++)
            {
                for (int xI = 0; xI < result.PixelWidth / lenth; xI++)
                {
                    Int32Rect rect = new Int32Rect(xI * lenth, yI * lenth, lenth, lenth);
                    result.WritePixels(rect, blocks[blockI], lenth * bytesPerPixel, 0);
                    blockI++;
                }
            }
            
            if (resized == true)
            {
                byte[] bytes = new byte[inputImg.PixelWidth * inputImg.PixelHeight * bytesPerPixel];
                result.CopyPixels(bytes, inputImg.PixelWidth * bytesPerPixel, 0);
                byte[] pixels = new byte[exportInfo[1] * exportInfo[2] * bytesPerPixel];
                int row = 0, col = 0, iteration = 1;
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = bytes[row * inputImg.PixelWidth * bytesPerPixel + col];
                    col++;
                    if (i == (exportInfo[1] * iteration * bytesPerPixel) - 1)
                    {
                        row++;
                        iteration++;
                        col = 0;
                    }
                }
                BitmapSource resizedImg = BitmapSource.Create(exportInfo[1], exportInfo[2], inputImg.DpiX, inputImg.DpiY, inputImg.Format, inputImg.Palette, pixels, exportInfo[1] * bytesPerPixel);
                return resizedImg;
            }
            return result;
        }
        private static byte[] EncryptBlocks(byte[] pixels)
        {
            int row, col;
            row = 0;
            while (row < lenth * bytesPerPixel)
            {
                col = 0;
                while (col <= (lenth - 1) * bytesPerPixel)
                {
                    byte tmp;
                    tmp = (byte)(pixels[row * lenth + col] ^ alphaK[col / bytesPerPixel]);
                    pixels[row * lenth + col] = (byte)(pixels[row * lenth + ((alphaWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel)] ^ alphaK[col / bytesPerPixel]);
                    pixels[row * lenth + ((alphaWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel)] = tmp;
                    if (bytesPerPixel > 1)
                    {
                        tmp = (byte)(pixels[row * lenth + col + 1] ^ redK[col / bytesPerPixel]);
                        pixels[row * lenth + col + 1] = (byte)(pixels[row * lenth + ((redWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 1] ^ redK[col / bytesPerPixel]);
                        pixels[row * lenth + ((redWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 1] = tmp;

                        tmp = (byte)(pixels[row * lenth + col + 2] ^ greenK[col / bytesPerPixel]);
                        pixels[row * lenth + col + 2] = (byte)(pixels[row * lenth + ((greenWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 2] ^ greenK[col / bytesPerPixel]);
                        pixels[row * lenth + ((greenWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 2] = tmp;

                        tmp = (byte)(pixels[row * lenth + col + 2] ^ blueK[col / bytesPerPixel]);
                        pixels[row * lenth + col + 3] = (byte)(pixels[row * lenth + ((blueWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 3] ^ blueK[col / bytesPerPixel]);
                        pixels[row * lenth + ((blueWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 3] = tmp;
                    }
                    col += bytesPerPixel;
                }
                row += bytesPerPixel;
            }
            col = 0;
            while (col < lenth * bytesPerPixel)
            {
                row = 0;
                while (row <= (lenth - 1) * bytesPerPixel)
                {
                    byte tmp;
                    tmp = (byte)(pixels[row * lenth + col] ^ alphaK[row / bytesPerPixel]);
                    pixels[row * lenth + col] = (byte)(pixels[row * lenth + ((alphaWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel)] ^ alphaK[row / bytesPerPixel]);
                    pixels[row * lenth + ((alphaWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel)] = tmp;
                    if (bytesPerPixel > 1)
                    {
                        tmp = (byte)(pixels[row * lenth + col + 1] ^ redK[row / bytesPerPixel]);
                        pixels[row * lenth + col + 1] = (byte)(pixels[row * lenth + ((redWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 1] ^ redK[row / bytesPerPixel]);
                        pixels[row * lenth + ((redWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 1] = tmp;

                        tmp = (byte)(pixels[row * lenth + col + 2] ^ greenK[row / bytesPerPixel]);
                        pixels[row * lenth + col + 2] = (byte)(pixels[row * lenth + ((greenWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 2] ^ greenK[row / bytesPerPixel]);
                        pixels[row * lenth + ((greenWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 2] = tmp;

                        tmp = (byte)(pixels[row * lenth + col + 3] ^ blueK[row / bytesPerPixel]);
                        pixels[row * lenth + col + 3] = (byte)(pixels[row * lenth + ((blueWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 3] ^ blueK[row / bytesPerPixel]);
                        pixels[row * lenth + ((blueWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 3] = tmp;
                    }
                    row += bytesPerPixel;
                }
                col += bytesPerPixel;
            }
            return pixels;
        }
        private static byte[] DecryptBlocks(byte[] pixels)
        {
            int row, col;
            col = (lenth - 1) * bytesPerPixel;
            while (col >= 0)
            {
                row = (lenth - 1) * bytesPerPixel;
                while (row >= 0)
                {
                    byte tmp;
                    tmp = (byte)(pixels[row * lenth + col] ^ alphaK[row / bytesPerPixel]);
                    pixels[row * lenth + col] = (byte)(pixels[row * lenth + ((alphaWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel)] ^ alphaK[row / bytesPerPixel]);
                    pixels[row * lenth + ((alphaWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel)] = tmp;
                    if (bytesPerPixel > 1)
                    {
                        tmp = (byte)(pixels[row * lenth + col + 1] ^ redK[row / bytesPerPixel]);
                        pixels[row * lenth + col + 1] = (byte)(pixels[row * lenth + ((redWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 1] ^ redK[row / bytesPerPixel]);
                        pixels[row * lenth + ((redWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 1] = tmp;

                        tmp = (byte)(pixels[row * lenth + col + 2] ^ greenK[row / bytesPerPixel]);
                        pixels[row * lenth + col + 2] = (byte)(pixels[row * lenth + ((greenWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 2] ^ greenK[row / bytesPerPixel]);
                        pixels[row * lenth + ((greenWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 2] = tmp;

                        tmp = (byte)(pixels[row * lenth + col + 3] ^ blueK[row / bytesPerPixel]);
                        pixels[row * lenth + col + 3] = (byte)(pixels[row * lenth + ((blueWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 3] ^ blueK[row / bytesPerPixel]);
                        pixels[row * lenth + ((blueWays[row / bytesPerPixel] ^ IV[col / bytesPerPixel]) * bytesPerPixel) + 3] = tmp;
                    }
                    row -= bytesPerPixel;
                }
                col -= bytesPerPixel;
            }
            row = (lenth - 1) * bytesPerPixel;
            while (row >= 0)
            {
                col = (lenth - 1) * bytesPerPixel;
                while (col >= 0)
                {
                    byte tmp;
                    tmp = (byte)(pixels[row * lenth + col] ^ alphaK[col / bytesPerPixel]);
                    pixels[row * lenth + col] = (byte)(pixels[row * lenth + ((alphaWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel)] ^ alphaK[col / bytesPerPixel]);
                    pixels[row * lenth + ((alphaWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel)] = tmp;
                    if (bytesPerPixel > 1)
                    {
                        tmp = (byte)(pixels[row * lenth + col + 1] ^ redK[col / bytesPerPixel]);
                        pixels[row * lenth + col + 1] = (byte)(pixels[row * lenth + ((redWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 1] ^ redK[col / bytesPerPixel]);
                        pixels[row * lenth + ((redWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 1] = tmp;

                        tmp = (byte)(pixels[row * lenth + col + 2] ^ greenK[col / bytesPerPixel]);
                        pixels[row * lenth + col + 2] = (byte)(pixels[row * lenth + ((greenWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 2] ^ greenK[col / bytesPerPixel]);
                        pixels[row * lenth + ((greenWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 2] = tmp;

                        tmp = (byte)(pixels[row * lenth + col + 3] ^ blueK[col / bytesPerPixel]);
                        pixels[row * lenth + col + 3] = (byte)(pixels[row * lenth + ((blueWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 3] ^ blueK[col / bytesPerPixel]);
                        pixels[row * lenth + ((blueWays[col / bytesPerPixel] ^ IV[row / bytesPerPixel]) * bytesPerPixel) + 3] = tmp;
                    }
                    col -= bytesPerPixel;
                }
                row -= bytesPerPixel;
            }
            return pixels;
        }
        public static List<int> GetWays(int from, int to, int lenth)
        {
            Random rnd = new Random();
            int way;
            List<int> array = new List<int>();
            array.Clear();
            while (array.Count != lenth)
            {
                way = rnd.Next(from, to);
                if (!array.Contains(way))
                {
                    array.Add(way);
                }
            }
            return array;
        }
        public static List<string> GetStrings()
        {
            List<string> array = new List<string>();
            array.Clear();
            while (array.Count != 256)
            {
                array.Add("0");
            }
            return array;
        }
    }
    #endregion
    /*class KeyTable
    {
        public KeyTable(string Type, string[] Keys)
        {
            this.Type = Type;
            this.Keys = Keys;
        }
        public string Type { get; set; }
        public string[] Keys { get; set; }
    }*/
    public partial class TranspositionAlg : UserControl
    {
        private static int method = 0;
        private static bool encrypted;
        public TranspositionAlg()
        {
            InitializeComponent();
        }
        private void SizeChanged(object sender, EventArgs e)
        {
            Encrypt.lenth = Int32.Parse((sender as ComboBox).Text);
        }
        private void MethodChanged(object sender, RoutedEventArgs e)
        {
            switch ((sender as RadioButton).Uid)
            {
                case "random":
                    method = 0;
                    allGen.Visibility = Visibility.Visible;

                    alphaGen.Visibility = Visibility.Visible;
                    redGen.Visibility = Visibility.Visible;
                    greenGen.Visibility = Visibility.Visible;
                    blueGen.Visibility = Visibility.Visible;

                    passGen.Visibility = Visibility.Collapsed;

                    alphaP.Visibility = Visibility.Collapsed;
                    redP.Visibility = Visibility.Collapsed;
                    greenP.Visibility = Visibility.Collapsed;
                    blueP.Visibility = Visibility.Collapsed;
                    break;
                case "pass":
                    method = 1;
                    allGen.Visibility = Visibility.Collapsed;

                    alphaGen.Visibility = Visibility.Collapsed;
                    redGen.Visibility = Visibility.Collapsed;
                    greenGen.Visibility = Visibility.Collapsed;
                    blueGen.Visibility = Visibility.Collapsed;

                    passGen.Visibility = Visibility.Visible;

                    alphaP.Visibility = Visibility.Visible;
                    redP.Visibility = Visibility.Visible;
                    greenP.Visibility = Visibility.Visible;
                    blueP.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void OpenPass(object sender, EventArgs e)
        {
            switch((sender as CheckBox).Name)
            {
                case "vAlpha":
                    (alphaPass.Child as System.Windows.Forms.TextBox).PasswordChar = Char.MinValue;
                    (alphaPass.Child as System.Windows.Forms.TextBox).Focus();
                    break;
                case "vRed":
                    (redPass.Child as System.Windows.Forms.TextBox).PasswordChar = Char.MinValue;
                    (redPass.Child as System.Windows.Forms.TextBox).Focus();
                    break;
                case "vGreen":
                    (greenPass.Child as System.Windows.Forms.TextBox).PasswordChar = Char.MinValue;
                    (greenPass.Child as System.Windows.Forms.TextBox).Focus();
                    break;
                case "vBlue":
                    (bluePass.Child as System.Windows.Forms.TextBox).PasswordChar = Char.MinValue;
                    (bluePass.Child as System.Windows.Forms.TextBox).Focus();
                    break;
            }
        }
        private void ClosePass(object sender, EventArgs e)
        {
            switch ((sender as CheckBox).Name)
            {
                case "vAlpha":
                    (alphaPass.Child as System.Windows.Forms.TextBox).PasswordChar = '•';
                    (alphaPass.Child as System.Windows.Forms.TextBox).Focus();
                    break;
                case "vRed":
                    (redPass.Child as System.Windows.Forms.TextBox).PasswordChar = '•';
                    (redPass.Child as System.Windows.Forms.TextBox).Focus();
                    break;
                case "vGreen":
                    (greenPass.Child as System.Windows.Forms.TextBox).PasswordChar = '•';
                    (greenPass.Child as System.Windows.Forms.TextBox).Focus();
                    break;
                case "vBlue":
                    (bluePass.Child as System.Windows.Forms.TextBox).PasswordChar = '•';
                    (bluePass.Child as System.Windows.Forms.TextBox).Focus();
                    break;
            }
        }
        private void Generate(object sender, RoutedEventArgs e)
        {
            byte[] key = new byte[Encrypt.lenth];
            RNGCryptoServiceProvider rNG = new RNGCryptoServiceProvider();
            rNG.GetBytes(key);
            switch ((sender as Button).Name)
            {
                case "allGen":
                    Encrypt.alphaK = key;
                    Encrypt.redK = key;
                    Encrypt.greenK = key;
                    Encrypt.blueK = key;

                    Encrypt.alphaWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    Encrypt.redWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    Encrypt.greenWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    Encrypt.blueWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);

                    Encrypt.test = Encrypt.GetStrings();
                    
                    break;
                case "alphaGen":
                    Encrypt.alphaK = key;
                    Encrypt.alphaWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    break;
                case "redGen":
                    Encrypt.redK = key;
                    Encrypt.redWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    break;
                case "greenGen":
                    Encrypt.greenK = key;
                    Encrypt.greenWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    break;
                case "blueGen":
                    Encrypt.blueK = key;
                    Encrypt.blueWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    break;
                case "passGen":
                    byte[] salt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

                    Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes((alphaPass.Child as System.Windows.Forms.TextBox).Text, salt);
                    Encrypt.alphaK = rfc.GetBytes(Encrypt.lenth);
                    rfc = new Rfc2898DeriveBytes((redPass.Child as System.Windows.Forms.TextBox).Text, salt);
                    Encrypt.redK = rfc.GetBytes(Encrypt.lenth);
                    rfc = new Rfc2898DeriveBytes((greenPass.Child as System.Windows.Forms.TextBox).Text, salt);
                    Encrypt.greenK = rfc.GetBytes(Encrypt.lenth);
                    rfc = new Rfc2898DeriveBytes((bluePass.Child as System.Windows.Forms.TextBox).Text, salt);
                    Encrypt.blueK = rfc.GetBytes(Encrypt.lenth);

                    Encrypt.alphaWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    Encrypt.redWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    Encrypt.greenWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    Encrypt.blueWays = Encrypt.GetWays(0, Encrypt.lenth, Encrypt.lenth);
                    break;
            }
        }
        public BitmapSource RunTransposition(BitmapSource uploadedImg, int action)
        {
            if (uploadedImg == null)
            {
                MessageBox.Show("Ви не завантажили зображення!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            BitmapSource returnedImg = null;
            switch (action)
            {
                case 0:
                    if (Encrypt.inBlock == true && Encrypt.bytesPerPixel > 1 && (Encrypt.alphaK == null || Encrypt.redK == null || Encrypt.greenK == null || Encrypt.blueK == null ||
                        Encrypt.alphaWays == null || Encrypt.redWays == null || Encrypt.greenWays == null || Encrypt.blueWays == null))
                    {
                        MessageBox.Show("Ви не згенерували ключі!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return null;
                    }
                    else if(Encrypt.inBlock == true && (Encrypt.alphaK == null || Encrypt.alphaWays == null))
                    {
                        MessageBox.Show("Ви не згенерували ключі!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return null;
                    }
                    returnedImg = Encrypt.EncryptImg(uploadedImg, method);
                    encrypted = true;
                    break;
                case 1:
                    if(encrypted == false)
                    {
                        MessageBox.Show("Ви вже розшифровували це зображення! Зашифруйте зображення знову для подальшої розшифровки!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return null;
                    }
                    returnedImg = Encrypt.InversEncryption(uploadedImg);
                    encrypted = false;
                    break;
            }
            return returnedImg;
        }
        public string ManageExport()
        {
            string key = "0," + Encrypt.exportInfo[0] + "," + String.Join(";", Encrypt.ways);
            if (Encrypt.inBlock == true)
            {
                key = key + ",1," + String.Join(";", Encrypt.IV);
                if (Encrypt.bytesPerPixel > 1)
                    key = key + ",1," + String.Join(";", Encrypt.alphaWays) + "," + String.Join(";", Encrypt.redWays) + "," +
                    String.Join(";", Encrypt.greenWays) + "," +
                    String.Join(";", Encrypt.blueWays);
                else
                    key = key + ",0," + String.Join(";", Encrypt.alphaWays);
                if (method == 0)
                {
                    if (Encrypt.bytesPerPixel > 1)
                        key = key + ",0," +
                                    String.Join(";", Encrypt.alphaK) + "," +
                                    String.Join(";", Encrypt.redK) + "," +
                                    String.Join(";", Encrypt.greenK) + "," +
                                    String.Join(";", Encrypt.blueK);
                    else
                        key = key + ",0," +
                                    String.Join(";", Encrypt.alphaK);
                }
                else
                    key = key + ",1";
                if (Encrypt.exportInfo.Count > 1)
                    key = key + ",1," + Encrypt.exportInfo[1] + "," + Encrypt.exportInfo[2];
                else
                    key = key + ",0";
            }
            else
            {
                key = key + ",0";
                if (Encrypt.exportInfo.Count > 1)
                    key = key + ",1," + Encrypt.exportInfo[1] + "," + Encrypt.exportInfo[2];
                else
                    key = key + ",0";
            }
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(key));
        }
        /*private void ShowTable(object sender, RoutedEventArgs e)
        {
            if(Encrypt.alphaK == null || Encrypt.redK == null || Encrypt.greenK == null || Encrypt.blueK == null ||
                        Encrypt.alphaWays == null || Encrypt.redWays == null || Encrypt.greenWays == null || Encrypt.blueWays == null)
            {
                MessageBox.Show("Ви не згенерували ключі!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            window.Cursor = Cursors.Wait;
            Table table = new Table();
            table.DrawTable(Encrypt.alphaWays, Encrypt.alphaK);
            table.Show();
            window.Cursor = Cursors.Arrow;
        }*/
        private void InBlocks(object sender, RoutedEventArgs e)
        {
            switch((sender as CheckBox).IsChecked)
            {
                case true:
                    Encrypt.inBlock = true;
                    break;
                case false:
                    Encrypt.inBlock = false;
                    break;
            }
        }
    }
}
