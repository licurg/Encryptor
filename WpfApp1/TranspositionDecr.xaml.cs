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
using System.IO;
using System.Security.Cryptography;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для TranspositionDecr.xaml
    /// </summary>

    public partial class TranspositionDecr : UserControl
    {
        public TranspositionDecr()
        {
            InitializeComponent();
        }

        public static int lenth;
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
        public static string method;
        private static int imgWidth;
        public static bool resized;
        private static int width;
        private static int height;
        public static bool inBlock;

        public BitmapSource DecryptImg(BitmapSource inputImg)
        {
            int bytesPerPixel = (inputImg.Format.BitsPerPixel + 7) / 8;
            if (inBlock && bytesPerPixel > 1 && (alphaK == null || redK == null || greenK == null || blueK == null ||
                       alphaWays == null || redWays == null || greenWays == null || blueWays == null))
            {
                MessageBox.Show("Ви не згенерували ключі!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            else if (inBlock && (alphaK == null || alphaWays == null))
            {
                MessageBox.Show("Ви не згенерували ключі!", "Застереження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            List<byte[]> blocks = new List<byte[]>();
            int blocksCount = (inputImg.PixelWidth * inputImg.PixelHeight) / (int)(Math.Pow(lenth, 2));
            for (int yI = 0; yI < inputImg.PixelHeight / lenth; yI++)
            {
                for (int xI = 0; xI < inputImg.PixelWidth / lenth; xI++)
                {
                    byte[] pixels = new byte[(int)(Math.Pow(lenth, 2)) * bytesPerPixel];
                    Int32Rect rect = new Int32Rect(xI * lenth, yI * lenth, lenth, lenth);
                    inputImg.CopyPixels(rect, pixels, lenth * bytesPerPixel, 0);
                    if (inBlock)
                        pixels = DecryptBlocks(pixels, bytesPerPixel);
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
                byte[] pixels = new byte[width * height * bytesPerPixel];
                int row = 0, col = 0, iteration = 1;
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i] = bytes[row * inputImg.PixelWidth * bytesPerPixel + col];
                    col++;
                    if (i == (width * iteration * bytesPerPixel) - 1)
                    {
                        row++;
                        iteration++;
                        col = 0;
                    }
                }
                BitmapSource resizedImg = BitmapSource.Create(width, height, inputImg.DpiX, inputImg.DpiY, inputImg.Format, inputImg.Palette, pixels, width * bytesPerPixel);
                return resizedImg;
            }
            return result;
        }
        private static byte[] DecryptBlocks(byte[] pixels, int bytesPerPixel)
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
        private void OpenPass(object sender, EventArgs e)
        {
            switch ((sender as CheckBox).Name)
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
            byte[] salt = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes((alphaPass.Child as System.Windows.Forms.TextBox).Text, salt);
            alphaK = rfc.GetBytes(lenth);
            rfc = new Rfc2898DeriveBytes((redPass.Child as System.Windows.Forms.TextBox).Text, salt);
            redK = rfc.GetBytes(lenth);
            rfc = new Rfc2898DeriveBytes((greenPass.Child as System.Windows.Forms.TextBox).Text, salt);
            greenK = rfc.GetBytes(lenth);
            rfc = new Rfc2898DeriveBytes((bluePass.Child as System.Windows.Forms.TextBox).Text, salt);
            blueK = rfc.GetBytes(lenth);
        }
        public bool ReadKey(string path)
        {
            string key = "";
            using (StreamReader file = new StreamReader(path, true))
            {
                key = Encoding.Unicode.GetString(Convert.FromBase64String(file.ReadLine()));
                string[] keys = key.Split(',');
                if (keys[0] != "0")
                {
                    
                    return false;
                }
                lenth = Int32.Parse(keys[1]);
                if(keys[2] != "")
                    ways = keys[2].Split(';').Select(Int32.Parse).ToList();
                if(keys[3] == "1")
                {
                    inBlock = true;
                    IV = keys[4].Split(';').Select(Int32.Parse).ToList();
                    if (keys[5] == "1")
                    {
                        alphaWays = keys[6].Split(';').Select(Int32.Parse).ToList();
                        redWays = keys[7].Split(';').Select(Int32.Parse).ToList();
                        greenWays = keys[8].Split(';').Select(Int32.Parse).ToList();
                        blueWays = keys[9].Split(';').Select(Int32.Parse).ToList();
                        if (keys[10] == "0")
                        {
                            alphaK = keys[11].Split(';').Select(Byte.Parse).ToArray();
                            redK = keys[12].Split(';').Select(Byte.Parse).ToArray();
                            greenK = keys[13].Split(';').Select(Byte.Parse).ToArray();
                            blueK = keys[14].Split(';').Select(Byte.Parse).ToArray();
                            if (keys[15] == "1")
                            {
                                resized = true;
                                width = Int32.Parse(keys[16]);
                                height = Int32.Parse(keys[17]);
                            }
                            else
                                resized = false;
                            return false;
                        }
                        else
                        {
                            if (keys[11] == "1")
                            {
                                resized = true;
                                width = Int32.Parse(keys[12]);
                                height = Int32.Parse(keys[13]);
                            }
                            else
                                resized = false;
                            return true;
                        }
                    }
                    else
                    {
                        alphaWays = keys[6].Split(';').Select(Int32.Parse).ToList();
                        if (keys[7] == "0")
                        {
                            alphaK = keys[8].Split(';').Select(Byte.Parse).ToArray();
                            if (keys[9] == "1")
                            {
                                resized = true;
                                width = Int32.Parse(keys[10]);
                                height = Int32.Parse(keys[11]);
                            }
                            else
                                resized = false;
                            return false;
                        }
                        else
                        {
                            if (keys[8] == "1")
                            {
                                resized = true;
                                width = Int32.Parse(keys[9]);
                                height = Int32.Parse(keys[10]);
                            }
                            else resized = false;
                            return true;
                        }
                    }
                }
                else
                {
                    inBlock = false;
                    if (keys[4] == "1")
                    {
                        resized = true;
                        width = Int32.Parse(keys[5]);
                        height = Int32.Parse(keys[6]);
                    }
                    return false;
                }
            }
        }
    }
}
